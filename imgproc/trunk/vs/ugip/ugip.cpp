#include "ugip.h"
#include "BufferPool.h"
#include "Buffer.h"
#include "Processor.h"
#include "Reader.h"
#include "Writer.h"
#include "Centers.h"
#include "ImageProcessorFactory.h"
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
#include <QtWidgets/QFileDialog>
#include <QtWidgets/QMessageBox>
#include <QtWidgets/QGraphicsItem>

/************************************************************************/
/*                    //TOTAL HACK JOB\\                                */
/************************************************************************/

#define qstrnum(_inttype) QString::number(_inttype) 

const int CIRCLESFILE_NUMLINES = 96+3;
using namespace uG;

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

// public SLOT
void ugip::circleAdded(QSelectableEllipse *el)
{   
    QPointF pf = el->scenePos();
    qDebug() << "added this circle: (x,y,r)=(" << pf.x() << "," << pf.y() << "," << uG_RADIUS << ")"  ;
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
    
    image = QImage(data8bit, uG_IMAGE_WIDTH, uG_IMAGE_HEIGHT, QImage::Format_Indexed8);
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

void ugip::scanFiles()
{

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
                    "[crad]:" << uG_RADIUS << "\n";            //circle radius

        int i=0;
        foreach(QGraphicsItem *item, circles) 
        {
            QPointF pf = item->pos();
            qDebug()<<pf;
            fileText << "[" % qstrnum(i) % "]:" 
                % qstrnum(pf.x()+uG_RADIUS) % ',' % qstrnum(pf.y()+uG_RADIUS) % '\n';
            i+=1;
        }
    } else { fileNotOpened( fileName ); return; }
    file.close(); 
}

void ugip::on_renderGreenCirclesCheckbox_toggled( bool checked )
{
    //TODO: implement ugip::on_renderGreenCirclesCheckbox_toggled( bool checked )
}

int ugip::parseCirclesFile( QString fileName )
{
    QFile file(fileName);
    file.open(QIODevice::ReadOnly|QIODevice::Text);
    QTextStream circles(&file);
    int line = m_circleCount = 0;

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


            //row=  k/uG::uG_CENTERS_COL_COUNT;
            //col=  k%uG::uG_CENTERS_COL_COUNT;
            uG::uGCenter c = { (double)x, (double)y, (double)m_radius };
            uG::uGcenter405[k] = c;
            ++m_circleCount;
            //std::cout << "About to set: " << row << "," << col << "," << x << "," << y << std::endl;
        }
        ++line;
    }
    //std::cout << "Returning..." << std::endl;
    return line;

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
        fileNotOpened(file.first());
    }
}

void ugip::on_beginProcessingButton_clicked()
{

}

void ugip::on_outputDirButton_clicked()
{

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
