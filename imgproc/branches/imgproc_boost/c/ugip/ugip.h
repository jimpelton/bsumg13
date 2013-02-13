#ifndef UGIP_H
#define UGIP_H

#include "QSelectableCircleScene.h"

#include "Centers.h"

#include <QtCore/QString>
#include <QtCore/QVector>
#include <QtWidgets/QMainWindow>
#include <QtWidgets/QGraphicsScene>
#include <QtGui/QImage>
#include <QtGui/QPixmap>
#include <QtCore/QStringList>

#include <vector>
#include <string>

using std::vector;
using std::string;

#include "ui_ugip.h"

class ugip : public QMainWindow
{
    Q_OBJECT

public:
    typedef QVector< uG::uGCenter > vcent;
    typedef QVector< vcent > vvcent;

    ugip(QWidget *parent = 0);
    
    ~ugip();

public slots:

    void circleAdded(QSelectableEllipse *);

private:
    Ui::ugipClass ui;
    
    QStringList m_fileNames;

    QString m_inputDirName, 
        m_circlesFile,
        m_outputDirName;

    QSelectableCircleScene *scene;
    
    QImage image;

    unsigned char *m_currentImageData;
    size_t m_currentDataSize;

    QPixmap pixmap;
    
    bool m_debugCircles, m_currentIs405; //< show debug circles
    
    int m_currentDisplayedFile,
        m_imgWidth, 
        m_imgHeight, 
        m_radius,  
        m_circleCount,
        m_numFiles;

    //open image at fileName and display with displaySingleData.
    void displaySingleImage(QString fileName);
    
    //convert _16bitData to 8bit and show as background in graphicsView.
    void displaySingleData(unsigned char *_16bitData, size_t szData);
    
    //open file at fileName and parse out the circle contents.
    int parseCirclesFile(QString fileName);

    //process the file at fileName, contents passed in as imgdata, results put into data.
    void processCurrentImage(bool is405, long long *data=NULL);

    unsigned char * openImage(QString fileName, size_t &szAlloc);

    void createPipeline();

    void findRows( QVector<uG::uGCenter> &centers, 
        QVector< QVector< uG::uGCenter > > &rows, 
        int dist_thresh);

    //error dialogs
    void fileNotOpened( QString fileName);
    void fileNotRead( QString fileName);
    void numberOfCirclesNot96();


    private slots:
        void on_browseButton_clicked();
        void on_beginProcessingButton_clicked();
        void on_outputDirButton_clicked();
        void on_scanFilesButton_clicked();
        void on_fileSelectSlider_sliderReleased();
        void on_renderDebugCirclesCheckbox_toggled(bool checked);
        void on_saveGreenCirclesButton_clicked();
        void on_renderGreenCirclesCheckbox_toggled(bool checked);
        void on_circlesFileBrowseButton_clicked();


};

#endif // UGIP_H
