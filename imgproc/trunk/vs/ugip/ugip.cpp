#include "ugip.h"
#include "BufferPool.h"
#include "Buffer.h"
#include "Processor.h"
#include "Reader.h"
#include "Writer.h"
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
#include <QtCore/QStringBuilder>
#include <QtWidgets/QFileDialog>
#include <QtWidgets/QMessageBox>
#include <QtWidgets/QGraphicsItem>


#define qstrnum(_inttype) QString::number(_inttype) 

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

}

ugip::~ugip() { }


void ugip::circleAdded(QSelectableEllipse *el)
{   
    QPointF pf = el->scenePos();
    qDebug() << "added this circle: (x,y,r)=(" << pf.x() << "," << pf.y() << "," << RADIUS << ")"  ;
    ui.graphicsView->update();
    
    ++m_circleCount;

    if (!ui.saveGreenCirclesButton->isEnabled())
        ui.saveGreenCirclesButton->setEnabled(true); 

}

void ugip::displaySingleImage( QString fileName )
{
    uG::AbstractImageProcessor *aip; 

    QFile img(fileName);
    img.open(QIODevice::ReadOnly);
    if (!img.isOpen()) { fileNotOpened(fileName); return; }

    QByteArray byteAray= img.readAll();
    size_t szRead = byteAray.length();
    if (szRead <= 0)   { fileNotRead(fileName); img.close(); return; }

    unsigned char *imgdata = (unsigned char*)(byteAray.data());
    long long data[96];

    if (fileName.contains("Camera405")) {

        aip = m_debugCircles ? new uG::CircleDrawingImageProcessor405(imgdata, data) 
                             : new uG::ImageProcessor405(imgdata,data); 

    } else {   //contains("Camera485")

        aip = m_debugCircles ? new uG::CircleDrawingImageProcessor485(imgdata, data)  
                             : new uG::ImageProcessor485(imgdata, data);    

    }
    aip->process();

    //convert 16bit data to 8bit 
    unsigned char *data8bit = new unsigned char[szRead/2];
    unsigned short *dsauce = (unsigned short*) imgdata;
    for (int i = 0; i<szRead/2; ++i) {
        data8bit[i] = (unsigned char) (dsauce[i]>>8);
    }
    
    image = QImage(data8bit, IMAGE_WIDTH, IMAGE_HEIGHT, QImage::Format_Indexed8);
    scene->setSceneRect(image.rect());
    ui.graphicsView->setBackgroundBrush(QPixmap::fromImage(image));
      
    ui.saveGreenCirclesButton->setEnabled(true);
    delete [] data8bit; //leaky leaky!!!
}

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
    QMessageBox::information(NULL, "Number of circles is different than 96!", "You have chosen to save a circles file that contains"\
        " more or less than 96 circles. Just sayin...");
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
        directoryName = files[0];
        ui.fileDirectoryLineEdit->setText(directoryName);
        ui.scanFilesButton->setEnabled(true);
    }
}

void ugip::on_scanFilesButton_clicked()
{
    ui.scanFilesButton->setEnabled(false);
    ui.progressBar->setEnabled(true);
    QDir dir;
    dir.setPath(directoryName);
    uint numfiles = dir.count();
    ui.fileDirectoryLineEdit->setText(directoryName);
    QDirIterator iter(dir.absolutePath(), QStringList() << "*.raw", QDir::Files);

    int found=0;
    while (iter.hasNext())
    {
        iter.next();
        fileNames.push_back(iter.fileName());
        ++found;
        ui.progressBar->setValue((found/(float)numfiles) * 100);
        ui.sliderMaxValueLabel->setText("/"+QString::number(found));
    }
    fileNames.sort();
    ui.progressBar->reset();
    ui.progressBar->setEnabled(false);

    ui.fileSelectSlider->setMaximum(found);
    ui.sliderValueSpinBox->setMaximum(found);
    m_currentDisplayedFile = 0;
    displaySingleImage(dir.absolutePath() + "/" + fileNames[0]);
}

void ugip::on_fileSelectSlider_sliderReleased()
{
    int filenum = ui.fileSelectSlider->value();
    if (filenum < fileNames.size()) {
        displaySingleImage(directoryName + "/" + fileNames[filenum]);
    }
}

void ugip::on_renderDebugCirclesCheckbox_toggled( bool checked)
{
    if (checked) m_debugCircles=true;
    else m_debugCircles=false;
    if (m_currentDisplayedFile >= 0 && m_currentDisplayedFile < fileNames.size()) {
        displaySingleImage(directoryName + "/" + fileNames[m_currentDisplayedFile]);
    }
}

void ugip::on_saveGreenCirclesButton_clicked()
{
    QList<QGraphicsItem*> circles = scene->items();

    if (circles.size() != 96) { numberOfCirclesNot96(); }

    QString fileName = QFileDialog::getSaveFileName(this, "Choose a name to save circles file as");

    QFile file(fileName);
    if (file.open(QIODevice::ReadWrite | QIODevice::Truncate))
    {
        QTextStream fileText(&file);

        fileText << "[imgx]:" << scene->width()  << "\n" <<    //image x
                    "[imgy]:" << scene->height() << "\n" <<    //image y
                    "[crad]:" << RADIUS << "\n";               //circle radius

        int i=0;
        foreach(QGraphicsItem *item, circles) 
        {
            QPointF pf = item->pos();
            qDebug()<<pf;
            fileText << "[" % qstrnum(i) % "]:" 
                % qstrnum(pf.x()+RADIUS) % ',' % qstrnum(pf.y()+RADIUS) % '\n';
            i+=1;
        }
    } else { fileNotOpened( fileName ); return; }
    file.close(); 
}

void ugip::on_renderGreenCirclesCheckbox_toggled( bool checked )
{
    //TODO: implement ugip::on_renderGreenCirclesCheckbox_toggled( bool checked )
}
