#include "uigp2.h"

#include "InfoWidget.h"
#include "SaveShowFrame.h"
#include "InputFrame.h"
#include "QSelectableCircleScene.h"

#include "CirclesFile.h"

#include <Imgproc.h>
#include <Centers.h>

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

    connect(m_saveShowFrame, SIGNAL(showDebugCircles(bool)),
        this, SLOT(drawDebugCircles(bool)));
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
    //	qDebug() << "Removed item: " << eee->x() << " " << eee->y();
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
    openCirclesFile();
}

void uigp2::addScannedFile( QString fname, float percentDone )
{
    m_fileNameList.push_back(fname);
    progressBar->setValue(percentDone*100.0f);
//    qDebug() << fname << ' ' << percentDone;
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
        QMessageBox::information(NULL, "No circles.",
                    "Please select an input circles file.");
    }
    //TODO: put circles into imgproc system.
    //TODO: setup imgproc pipeline.
}

void uigp2::saveCirclesFile(QString fname)
{
    if (fname.isEmpty()) {
        qDebug() << "No circles file name given to save to.";
        return;
    }

    m_circlesFile = CirclesFile(fname.toStdString());

    CirclesFile::ImageInfo imgInfo =
    { 
        m_currentImage.byteCount(), 
        m_currentImage.width(), 
        m_currentImage.height() 
    };

    //get all items from scene, then remove non-ellipse items.
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
    
    int rval = m_circlesFile.writeCirclesFile(v.toStdVector(), imgInfo);

    qDebug() << "Wrote " << rval << " circles" ;

    //TODO: open newly saved circles dialog and display circles.
}

void uigp2::openCirclesFile()
{
    if (m_circlesFileName.isEmpty()) {
        qDebug() << "No filename for circles file.";
        return;
    } 

    m_circlesFile = CirclesFile(m_circlesFileName.toStdString());
    int numCirc = m_circlesFile.open();
    if (!m_circlesFile.isOpen()){
        qDebug() << "openCirclesFile: CirclesFile reported it could not" \
                    " open the given circles file.";

        return;
    }

    m_scene->clear();
    m_scene->update();

    if (m_circlesList.size() < numCirc) {
        m_circlesList.resize(numCirc);
    }

    for (int i = 0; i < numCirc; ++i) {
        m_circlesList.replace(i, m_circlesFile.getCenter(i));
    }

    int rad = m_circlesFile.getRadius();
    for (int i = 0; i < m_circlesList.size(); ++i)
    {
        CirclesFile::CenterInfo info = m_circlesList[i];

        QSelectableEllipse *circle =
                new QSelectableEllipse(0, 0, 2*rad, 2*rad);

        circle->setPen(QPen(Qt::green));
        circle->setZValue(100);
        circle->setVisible(true);
        m_scene->addItem(circle);

        circle->setPos(info.x - rad, info.y - rad);
    }

    m_scene->update();

    m_scene->rGetRadius() = rad;
    m_circlePropsFrame->horizontalSlider->setValue(rad);
    m_circlePropsFrame->spinBox->setValue(rad);
    m_saveShowFrame->showGreenCirclesCheckbox->setChecked(true);
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
    QByteArray byteAray = img.readAll();
    img.close();

    size_t szAlloc = byteAray.length();
    if (szAlloc <= 0)   { 
        //fileNotRead(fileName); 
        return; 
    }

    unsigned char *grey_16 = (unsigned char *)byteAray.data();
    displayImageFromBytes(grey_16, szAlloc);

}

void uigp2::doneScan()
{
    qDebug() << "Done scan.";
    m_fileNameList.sort();
    displayImage(0);
}

void uigp2::drawDebugCircles(bool tog)
{
    if (!m_circlesFile.isOpen()){
        openCirclesFile();
        if (!m_circlesFile.isOpen()){
            qDebug() << "Can't draw debug circles because the circles " \
                        "file reported that it wasn't open.";
            return;
        }
    }

    //TODO: toggle debug circles off.

    uG::Imgproc img;
    int ncirc = m_circlesFile.getNumCircles();
    uG::uGProcVars *vars = img.newProcVars(ncirc);
    vars->imgh = 1944;
    vars->imgw = 2592;
    vars->radius = m_circlesFile.getRadius();
    vars->numWells = ncirc;
    vars->procType = uG::DEBUG_CIRCLES;
    for (int i = 0; i < vars->numWells; ++i) {
        CirclesFile::CenterInfo ci = m_circlesFile.getCenter(i);
        uG::uGCenter ugc = {ci.x, ci.y, ci.r};
        vars->centers[i] = ugc;
    }

    unsigned char *buf=NULL;
    string f = m_fileNameList[0].toStdString();
    size_t szAlloc = img.getBlackCirclesImage(f, *vars, &buf);



    displayImageFromBytes(buf, szAlloc);


    qDebug() << "Draw debug circles.";
}


void uigp2::displayImageFromBytes(unsigned char *raw16bit, size_t szAlloc)
{
     //convert 16bit data to 8bit
    unsigned char *data8bit = new unsigned char[szAlloc/2];
    unsigned short *dsauce = (unsigned short*) raw16bit;
    for (int i = 0; i<szAlloc/2; ++i) {
        data8bit[i] = (unsigned char) (dsauce[i]>>8);
    }

    QImage img(data8bit,2592,1944, QImage::Format_Indexed8);
    displayImageFromImage(img);

    delete [] data8bit;
}

void uigp2::displayImageFromImage(QImage img)
{
    m_lastImage = m_currentImage;
    m_currentImage = img;
    graphicsView->setBackgroundBrush(QPixmap::fromImage(img));
    graphicsView->update();
    m_scene->update(m_scene->sceneRect());

}

