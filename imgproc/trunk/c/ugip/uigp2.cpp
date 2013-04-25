#include "uigp2.h"

#include "InfoWidget.h"
#include "SaveShowFrame.h"
#include "InputFrame.h"
#include "QSelectableCircleScene.h"

#include "CirclesFile.h"

#include <QtCore/QDebug>
#include <QtWidgets/QGridLayout>
#include <QtWidgets/QSizePolicy>
#include <QtWidgets/QMessageBox>
#include <QtCore/QString>
#include <QtCore/QFile>

#include <list>

uigp2::uigp2(QWidget *parent)
    : QMainWindow(parent)
{
    setupUi(this);
   
    m_scene = new QSelectableCircleScene();
    m_scene->setSceneRect(0,0,2592,1944);
    this->graphicsView->setScene(m_scene);

    m_saveShowFrame = infoWidget->getSaveShowFrame();
    m_inputFrame = infoWidget->getInputFrame();
	m_circlePropsFrame = infoWidget->getCirclePropsFrame();	

    connect(m_scene, SIGNAL(insertedNewCircle(QSelectableEllipse*)), 
        this, SLOT(circleAdded(QSelectableEllipse*)));

	connect(m_circlePropsFrame, SIGNAL(circleRadiusChanged(int)), 
		this, SLOT(changeCircleRadius(int)));

    connect(m_inputFrame, SIGNAL(imageDir(QString)), 
        this, SLOT(setImageDir(QString)));

    connect(m_inputFrame, SIGNAL(outputDir(QString)),
        this, SLOT(setOutputDir(QString)));

    connect(m_inputFrame, SIGNAL(circlesFileName(QString)),
        this, SLOT(setCirclesFileName(QString)));

    connect(m_inputFrame, SIGNAL(scannedFile(QString, float)),
        this, SLOT(addScannedFile(QString, float)));

    connect(m_inputFrame, SIGNAL(startProcessing()),
        this, SLOT(startProcessing()));

    connect(m_inputFrame, SIGNAL(doneScan()), 
        this, SLOT(doneScan()));

    connect(m_saveShowFrame, SIGNAL(saveCircles(QString)),
        this, SLOT(saveCirclesFile(QString)));
    
}

uigp2::~uigp2() { }

/************************************************************************/
/*    PUBLIC SLOTS                                                      */
/************************************************************************/

void uigp2::circleAdded(QSelectableEllipse *eee)
{
	//TODO: increment circle count?
}

void uigp2::addSelectedCircle( QSelectableEllipse* eee ) 
{
    CirclesFile::CenterInfo c;
    c.r = eee->rad();
    c.x = eee->x()+c.r;
    c.y = eee->y()+c.r;

    m_circlesList.push_back(c);
    qDebug() << "added Center " << c.x << "," << c.y << "," << c.r;
}

void uigp2::removedExistingCircle(QGraphicsItem *eee)
{
	qDebug() << "Removed item: " << eee->x() << " " << eee->y();	
}

void uigp2::changeCircleRadius(int r)
{
	m_scene->setRadius(r);	
	qDebug() << "Circle radius change: " << r;
}

void uigp2::setImageDir( QString dir )
{
    m_imageDir = dir;
    qDebug() << "setImageDir: " << dir;
}

void uigp2::setOutputDir( QString dir )
{
    m_outputDir = dir;
    qDebug() << "setOutputDir: " << dir;
}

void uigp2::setCirclesFileName( QString fname )
{
    m_circlesFileName = fname;
    qDebug() << "setCirclesFileName: " << fname;
    //int errnum = parseCirclesFile(fname.toStdString());
    //if (errnum < 0){
    //    qDebug() << "parseCirclesFile() returned -1.";
    //}
}

void uigp2::addScannedFile( QString fname, float percentDone )
{
    m_fileNameList.push_back(fname);
    progressBar->setValue(percentDone*100.0f);
    qDebug() << fname << ' ' << percentDone;
}

void uigp2::startProcessing()
{
    qDebug() << "start proc'in.";
    if (m_fileNameList.size() < 1) {
        qDebug() << "Ahnooo...No file names in the list.";
        QMessageBox::information(NULL, "No file names.", "Please select an input" \
			"directory with some .raw files in it.");
    }
    if (m_circlesList.size() < 1) {
        qDebug() << "Ahnooo...No circles in the list.";
        QMessageBox::information(NULL, "No circles.", "Please select an input circles file.");
    }
    //TODO: put circles into imgproc system.
    //TODO: setup imgproc pipeline.
}

void uigp2::saveCirclesFile(QString fname)
{
    if (fname.isEmpty()) {
        //TODO: write with default filename.
        qDebug() << "No circles file name given to save to.";
        return;
    }

    m_circlesFile = CirclesFile(fname.toStdString());

    CirclesFile::ImageInfo img = 
    { 
        m_currentImage.byteCount(), 
        m_currentImage.width(), 
        m_currentImage.height() 
    };

    //get all items, then remove non-ellipse items.
    QVector<CirclesFile::CenterInfo> v;
    QList<QGraphicsItem*> thangs = m_scene->items();

    for (int i=0; i < thangs.size(); ++i) {
        QGraphicsItem *item = thangs.at(i);

        //remove any non-ellipse items
        if (item->type() == QSelectableEllipse::Type) {
            QSelectableEllipse *eee =
                    qgraphicsitem_cast<QSelectableEllipse*>(item);

            if (!eee) { continue; }

            int rad = eee->rad();
            CirclesFile::CenterInfo c(
                    eee->x()+rad,
                    eee->y()+rad,
                    rad);
            v.push_back(c);
        }  
    } // for 
    
    int rval = m_circlesFile.writeCirclesFile(v.toStdVector(), img );

    qDebug() << "Wrote " << rval << " circles";
}

void uigp2::openCirclesFile()
{
    if (m_circlesFileName.isEmpty()) {
        qDebug() << "No filename for circles file.";
        return;
    } 

    m_circlesFile = CirclesFile(m_circlesFileName.toStdString());

    if (m_circlesList.size() < m_circlesFile.open()) {
        m_circlesList.resize(m_circlesFile.getNumCircles());
    }

    for (int i = 0; i < m_circlesFile.getNumCircles(); ++i) {
        m_circlesList.replace(i, m_circlesFile.getCenter(i));
    }

    //TODO: draw circles?
}

void uigp2::displayImage( int idx )
{
	if (m_fileNameList.isEmpty())
	{
		qDebug() << "Filename list is empty, can't display image.";
		return;
	}
    if (idx >= m_fileNameList.size() || idx < 0) {
        qDebug() << "Invalid index given to display image: " << idx;
        return;
    }

    //open image file
    QString thang = m_fileNameList.at(0);
    QFile img(thang);
    img.open(QIODevice::ReadOnly);
    if (!img.isOpen()) { 
        //fileNotOpened(fileName); 
        qDebug() << "Couldn't open image: " << idx << ' ' << thang;
        return;
    }

    //read image into byte array
    QByteArray byteAray= img.readAll();
    size_t szAlloc = byteAray.length();
    if (szAlloc <= 0)   { 
        //fileNotRead(fileName); 
        img.close(); 
        return; 
    }
    
    //convert 16bit data to 8bit 
    unsigned char *data8bit = new unsigned char[szAlloc/2];
    unsigned short *dsauce = (unsigned short*) byteAray.data();
    for (int i = 0; i<szAlloc/2; ++i) {
        data8bit[i] = (unsigned char) (dsauce[i]>>8);
    }
    
    //give graphics view a pixmap to display as background.
    m_currentImage = QImage(data8bit, 2592, 1944, QImage::Format_Indexed8);
    m_scene->setSceneRect(m_currentImage.rect());
    graphicsView->setBackgroundBrush(QPixmap::fromImage(m_currentImage));

    delete [] data8bit;
}

void uigp2::doneScan()
{
    qDebug() << "Done scan.";
    displayImage(0);
}


