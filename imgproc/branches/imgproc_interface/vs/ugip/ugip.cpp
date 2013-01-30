#include "ugip.h"
#include "BufferPool.h"
#include "Buffer.h"
#include "Processor.h"
#include "Reader.h"
#include "Writer.h"
#include "Centers.h"
#include "WorkerThread.h"
#include "AbstractImageProcessor.h"
#include "ImageProcessor405.h"
#include "ImageProcessor485.h"
#include "CircleDrawingImageProcessor405.h"
#include "CircleDrawingImageProcessor485.h"
#include "ugTypes.h"

#include <QtCore/QDebug>
#include <QtCore/QDir>
#include <QtCore/QDirIterator>
#include <QtCore/QFile>
#include <QtCore/QList>
#include <QtCore/QRegExp>
#include <QtCore/QString>
#include <QtCore/QStringBuilder>
#include <QtCore/QVector>
#include <QtWidgets/QFileDialog>
#include <QtWidgets/QMessageBox>
#include <QtWidgets/QGraphicsItem>

//just wouldn't be complete w/o these stl classics!
#include <vector>
#include <string>

/************************************************************************/
/*                    //TOTAL HACK JOB\\                                */
/*    Note that this is experimental code to learn Qt and help          */
/*    develop the imgproc interface into something that is at least     */ 
/*    somewhat gui-friendly. //  [1/24/2013 jim]                        */  
/************************************************************************/

#define qstrnum(_inttype) QString::number(_inttype) 

using uG::BufferPool;
using uG::Processor;
using uG::Writer;
using uG::Reader;
using uG::WorkerThread;

using std::string;
using std::vector;

/************************************************************************/
/*   CONST DEFINITIONS                                                  */
/************************************************************************/

const int NUM_READERS = 4;
const int NUM_PROCS   = 2;
const int NUM_WRITE   = 2;
const int CIRCLESFILE_NUMLINES = 96+3;

bool sortCenterByX(const uG::uGCenter &lhs, const uG::uGCenter &rhs)
{
    return lhs.x < rhs.x;
}

bool sortCenterByY(const uG::uGCenter &lhs, const uG::uGCenter &rhs)
{
    return lhs.y < rhs.y;
}


/************************************************************************/
/*        CLASS IMPLEMENTATION                                          */
/************************************************************************/

ugip::ugip(QWidget *parent)
    : QMainWindow(parent)
    , m_debugCircles(false)
    , m_currentDisplayedFile(-1)
{
    ui.setupUi(this);
    ui.progressBar->setEnabled(false);

    scene = new QSelectableCircleScene();
    scene->setSceneRect(0,0,2592,1944);
    ui.graphicsView->setScene(scene);

    connect(scene, SIGNAL( insertedNewCircle(QSelectableEllipse*) ),
            this,  SLOT( circleAdded(QSelectableEllipse*) ));


    m_currentImageData = NULL;
    m_currentIs405=false;
}

ugip::~ugip() { }

// public SLOT // callback from the graphics scene.
void ugip::circleAdded(QSelectableEllipse *el)
{   
    QPointF pf = el->scenePos();
    qDebug() << "added this circle: (x,y,r)=(" << pf.x() << "," << pf.y() << "," << uG::uG_RADIUS << ")"  ;
    ui.graphicsView->update();
    
    ++m_circleCount;

    if (!ui.saveGreenCirclesButton->isEnabled())
        ui.saveGreenCirclesButton->setEnabled(true); 

}

unsigned char* ugip::openImage(QString fileName, size_t &szAlloc)
{
    unsigned char *rval = NULL;

    m_currentIs405 = fileName.contains("Camera405");

    //open
    QFile img(fileName);
    img.open(QIODevice::ReadOnly);
    if (!img.isOpen()) { fileNotOpened(fileName); return NULL; }

    //read
    QByteArray byteAray= img.readAll();
    szAlloc = byteAray.length();
    m_currentDataSize=szAlloc;
    if (szAlloc <= 0)   { fileNotRead(fileName); img.close(); return NULL; }
    
    rval = new unsigned char[szAlloc];
    memcpy(rval, byteAray.data(), szAlloc);

    if (m_currentImageData) delete [] m_currentImageData;
    m_currentImageData = rval;

    return rval;
}

void ugip::displaySingleImage( QString fileName )
{
    size_t szRead;
    unsigned char *imgdata = openImage(fileName, szRead);
    displaySingleData(imgdata, szRead);
    //delete [] imgdata;
}

void ugip::displaySingleData(unsigned char *_16bitData, size_t szData)
{
    //convert 16bit data to 8bit 
    unsigned char *data8bit = new unsigned char[szData/2];
    unsigned short *dsauce = (unsigned short*) _16bitData;
    for (int i = 0; i<szData/2; ++i) {
        data8bit[i] = (unsigned char) (dsauce[i]>>8);
    }
    
    image = QImage(data8bit, uG::uG_IMAGE_WIDTH, uG::uG_IMAGE_HEIGHT, QImage::Format_Indexed8);
    scene->setSceneRect(image.rect());
    ui.graphicsView->setBackgroundBrush(QPixmap::fromImage(image));

    delete data8bit;
}

void ugip::processCurrentImage(bool is405, long long *data)
{
    long long mydat[96];
    if (data==NULL){
        data = mydat;
    }

    unsigned char *imgdata = m_currentImageData;
    uG::AbstractImageProcessor *aip; 
    if (is405) {

        aip = m_debugCircles ? new uG::CircleDrawingImageProcessor405(imgdata, data) 
                             : new uG::ImageProcessor405(imgdata,data); 

    } else {   //contains("Camera485")

        aip = m_debugCircles ? new uG::CircleDrawingImageProcessor485(imgdata, data)  
                             : new uG::ImageProcessor485(imgdata, data);    

    }
    aip->process();
}

/************************************************************************/
/*   PRIVATE SLOTS                                                      */
/************************************************************************/

void ugip::on_browseButton_clicked()
{
    QFileDialog dialog(this);
    dialog.setFileMode(QFileDialog::Directory);
    dialog.setOption(QFileDialog::ShowDirsOnly);

    QStringList files;
    if (dialog.exec())
        files = dialog.selectedFiles();

    if (files.size() > 0) {
        m_inputDirName = files[0];
        ui.fileDirectoryLineEdit->setText(m_inputDirName);
        ui.scanFilesButton->setEnabled(true);
    }
}

void ugip::on_scanFilesButton_clicked()
{
    ui.scanFilesButton->setEnabled(false);
    ui.progressBar->setEnabled(true);
    QDir dir;
    dir.setPath(m_inputDirName);
    uint numfiles = m_numFiles = dir.count();
    ui.fileDirectoryLineEdit->setText(m_inputDirName);
    QDirIterator iter(dir.absolutePath(), QStringList() << "*.raw", QDir::Files);

    int found=0;
    while (iter.hasNext())
    {
        iter.next();
        m_fileNames.push_back(iter.fileName());
        ++found;
        ui.progressBar->setValue((found/(float)numfiles) * 100);
        ui.sliderMaxValueLabel->setText("/" % QString::number(found));
    }
    m_fileNames.sort();
    ui.progressBar->reset();
    ui.progressBar->setEnabled(false);

    ui.fileSelectSlider->setMaximum(found);
    ui.sliderValueSpinBox->setMaximum(found);
    m_currentDisplayedFile = 0;
    displaySingleImage(dir.absolutePath() % "/" % m_fileNames[0]);
    ui.circlesFileBrowseButton->setEnabled(true);

}

void ugip::on_fileSelectSlider_sliderReleased()
{
    int filenum = ui.fileSelectSlider->value();
    if (filenum < m_fileNames.size()) {
        displaySingleImage(m_inputDirName % "/" % m_fileNames[filenum]);
    }
}

void ugip::on_renderDebugCirclesCheckbox_toggled( bool checked)
{
    if (checked) m_debugCircles=true;
    else m_debugCircles=false;
    if (m_currentDisplayedFile >= 0 && m_currentDisplayedFile < m_fileNames.size()) {
        processCurrentImage(m_currentIs405);
        displaySingleData(m_currentImageData, m_currentDataSize );
    }
}

void ugip::on_saveGreenCirclesButton_clicked()
{
    QList<QGraphicsItem*> circles = scene->items();

    for (int i = 0; i<circles.size(); ++i)
    {
        if (circles[i]->type() != QSelectableEllipse::Type)
        {
            circles.removeAt(i);
            qDebug() << "Removed non-ellipse item @ idx: " << i;
        }
    }

    if (circles.size() != 96) { numberOfCirclesNot96(); }

    QString fileName = QFileDialog::getSaveFileName(this, "Choose a name to save circles file as");

    QFile file(fileName);
    if (file.open(QIODevice::ReadWrite | QIODevice::Truncate))
    {
        QTextStream fileText(&file);

        fileText << "[imgx]:" << scene->width()  << "\n" <<    //image x dim
                    "[imgy]:" << scene->height() << "\n" <<    //image y dim
                    "[crad]:" << uG::uG_RADIUS << "\n";            //circle radius

        int i=0;
        foreach(QGraphicsItem *item, circles) 
        {
            QPointF pf = item->pos();
            qDebug()<<pf;
            fileText << "[" % qstrnum(i) % "]:" 
                % qstrnum(pf.x()+uG::uG_RADIUS) % ',' % qstrnum(pf.y()+uG::uG_RADIUS) % '\n';
            i+=1;
        }
    } else { fileNotOpened( fileName ); return; }
    file.close(); 
}

void ugip::on_renderGreenCirclesCheckbox_toggled( bool checked )
{
    throw std::exception("on_renderGreenCirclesCheckbox_toggled not implemented.");
}

void ugip::on_circlesFileBrowseButton_clicked()
{
    QFileDialog dialog(this);
    dialog.setFileMode(QFileDialog::ExistingFile);
    QStringList file;
    if (dialog.exec())
        file = dialog.selectedFiles();

    if (file.size()>0) {
        m_circlesFile = file.front();
        int lines = parseCirclesFile(m_circlesFile);
        qDebug() << "Parsed " << lines << "lines, " << m_circleCount << " circles.";
        ui.circlesFileLineEdit->setText(m_circlesFile);
        processCurrentImage(m_currentIs405);
        ui.renderDebugCirclesCheckbox->setEnabled(true);
        ui.beginProcessingButton->setEnabled(true);
        ui.renderGreenCirclesCheckbox->setEnabled(true);
    } else {
        fileNotOpened("");
    }
}

void ugip::on_beginProcessingButton_clicked()
{
    createPipeline();
}

void ugip::on_outputDirButton_clicked()
{
    QFileDialog dialog(this);
    dialog.setFileMode(QFileDialog::DirectoryOnly);
    QStringList fileName;
    if (dialog.exec())
        fileName = dialog.selectedFiles();
    m_outputDirName = fileName.first();
}

/************************************************************************/
/*   PRIVATE HELPERS                                                   */
/************************************************************************/

int ugip::parseCirclesFile( QString fileName )
{
    QFile file(fileName);
    file.open(QIODevice::ReadOnly|QIODevice::Text);
    QTextStream circles(&file);
    int line = m_circleCount = 0;
    vcent centers;
    //[xx]|[abcd]:[xxxx],[xxxx]
    QRegExp regex("^\\[(\\d\\d?|[a-zA-Z]{4})\\]:([0-9]{1,4}),?([0-9]{0,4})$");


    while (!circles.atEnd() && line < CIRCLESFILE_NUMLINES)
    {
        QString s = circles.readLine();
        //qDebug() << line <<" String: " << s;
        if (regex.indexIn(s)!=0) {
            qDebug() << "Couldn't match with regex!"; 
            break;
        }
        QString key = regex.cap(1);
        QString val = regex.cap(2);
        QString val2 = regex.cap(3);
        bool ok;
        if (val2.isEmpty()) //hmm... seems a bit sketchy...
        {

            if (key == "imgx") {
                m_imgWidth = val.toInt(&ok);
                if (!ok) { qDebug() << "Couldn't convert val to m_imgWidth."; break; }
            } else if (key == "imgy") {
                m_imgHeight = val.toInt(&ok);
                if (!ok) { qDebug() << "Couldn't convert val to m_imgHeight."; break; }
            } else if (key == "crad") { 
                m_radius = val.toInt(&ok);
                if (!ok) { qDebug() << "Couldn't convert val to m_radius."; break; }
            }
            //qDebug() << "val2 was empty so I set something else.";
        }
        else
        {

            int x,y,k,row,col;

            x = val.toInt(&ok);
            //qDebug() << "x=" << x;
            if (!ok) { qDebug() << "Couldn't convert val."; break; }

            y = val2.toInt(&ok);
            //qDebug() << "y=" << y;
            if (!ok) { qDebug() << "Couldn't convert val2."; break; }

            k=key.toInt(&ok); 
            //qDebug() << "k=" << k;
            if (!ok) { qDebug() << "Couldn't convert key."; break; }


            uG::uGCenter c = { (double)x, (double)y, (double)m_radius };
            centers.push_back(c);
            //centers[k].x = c.x;
            //centers[k].y = c.y;
            //centers[k].r = c.r;

            ++m_circleCount;
            //std::cout << "About to set: " << row << "," << col << "," << x << "," << y << std::endl;
        }
        ++line;
    }

    //sort and copy to imgproc system.
    vvcent rows;
    findRows(centers, rows, 45);
    int idx=0;
    foreach(vcent r, rows)
    {
        foreach(uG::uGCenter c, r)
        {
            uG::uGcenter405[idx] = c;
            idx++;
        }
    }
    //std::cout << "Returning..." << std::endl;
    return line;
}

void ugip::findRows( QVector<uG::uGCenter> &centers, 
    QVector< QVector< uG::uGCenter > > &rows, 
    int dist_thresh)
{
    if (rows.size() < 8) { rows.reserve(8); }

    qSort(centers.begin(), centers.end(), sortCenterByY);

	vcent::iterator centerIt = centers.begin();
	vcent::iterator centerIt_end = centers.end()-1;

    int curRow = 0;	
	while (centerIt != centerIt_end) 
	{
        //check for y diff that is large enough to be on a new row        
		if ( qAbs( (*centerIt).y - (*(centerIt+1)).y ) > dist_thresh) 
        {
            vcent aRow = rows[curRow];
            qSort(aRow.begin(), aRow.end(), sortCenterByX);
			aRow.push_back(*centerIt);
            curRow+=1;
            if (curRow == rows.size()) break; //on last row.
		}
		else { rows[curRow].push_back(*centerIt); }
        ++centerIt;
	}
}

/************************************************************************/
/*                   LITTLE DIALOGS                                     */ 
/************************************************************************/
void ugip::fileNotOpened( QString fileName)
{
    QMessageBox::information(NULL, "Couldn't open files.", "The file " + fileName + " couldn't be opened.");
}

void ugip::fileNotRead( QString fileName)
{
    QMessageBox::information(NULL, "Couldn't read files.", "The file " + fileName + " couldn't be read.");
}

void ugip::numberOfCirclesNot96()
{
    QMessageBox::information(NULL, 
        "Number of circles is different than 96!", 
        "You have chosen to save a circles file that contains"\
        " more or less than 96 circles. Just sayin...");
}


/************************************************************************/
/*        CREATE PIPELINE                                               */
/************************************************************************/

void ugip::createPipeline()
{
    typedef vector<string> stringVector; 
    //vector madness!
    vector<stringVector> readerFilesVec(NUM_READERS);
    vector<Reader*> readers;
    vector<Processor*> procs;
    vector<Writer*> writers;
    vector<WorkerThread*> workers;    

    uG::ImageBufferPool imgbp(60, m_currentDataSize);
    uG::DataBufferPool  datbp(20, 96);

    //partition file names for Readers
    int i = 0;
    QStringList::iterator it = m_fileNames.begin();
    while ( it != m_fileNames.end() ) {
        QString f = m_inputDirName % '/' % *it;
        readerFilesVec.at(i%NUM_READERS).push_back(f.toStdString());
        ++it; ++i;
    }
    
    for (int i = 0; i < NUM_READERS; ++i)
    {
        Reader *r = new Reader(readerFilesVec[i], &imgbp);
        readers.push_back(r);
        workers.push_back(new WorkerThread(r->do_work, (void*)r));
    }

    for (int i = 0; i < NUM_PROCS; ++i)
    {
        Processor *p = new Processor(&imgbp, &datbp);
        procs.push_back(p);
        workers.push_back(new WorkerThread(p->do_work, (void*)p));
    }

    string outpath = m_outputDirName.toStdString() + "/";
    for (int i = 0; i < NUM_WRITE; ++i)
    {
        Writer *w = new Writer(outpath, &datbp);
        writers.push_back(w);
        workers.push_back(new WorkerThread(w->do_work, (void*)w));
    }

     
    std::cout << "Created: " << readers.size() << " reader threads.\n";
    std::cout << "Created: " << procs.size() << " processor threads.\n";
    std::cout << "Created: " << writers.size() << " writer threads.\n";
    std::cout << "Starting threads..." << std::endl;

    vector<WorkerThread*>::iterator worker_iter = workers.begin();
    for (; worker_iter != workers.end(); ++worker_iter)
    {
        (*worker_iter)->go();
    }

    worker_iter = workers.begin();
    for(; worker_iter != workers.end(); ++worker_iter)
    {
        (*worker_iter)->join();
    }
}



