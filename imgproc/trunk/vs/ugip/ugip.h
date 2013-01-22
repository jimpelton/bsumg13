#ifndef UGIP_H
#define UGIP_H

#include "QSelectableCircleScene.h"

#include <QtCore/QString>
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
    ugip(QWidget *parent = 0);
    
    ~ugip();

public slots:

    void circleAdded(QSelectableEllipse *);

private:
    Ui::ugipClass ui;
    
    QStringList fileNames;

    QString directoryName;

    QSelectableCircleScene *scene;
    
    QImage image;
    
    QPixmap pixmap;

    //QList<CircleCoordinates> m_circleCoords;

    //QSet<CircleCoordinates> m_circleCoords;

    int m_circleCount;
    
    bool m_debugCircles;
    
    int m_currentDisplayedFile;

    int m_imgWidth, m_imgHeight, m_radius;
    double **m_centersX, **m_centersY;

    void displaySingleImage(QString fileName);

    //error dialogs
    void fileNotOpened( QString fileName);
    void fileNotRead( QString fileName);
    void numberOfCirclesNot96();

    void parseCirclesFile(QString fileName);

    private slots:
        void on_browseButton_clicked();
        void on_scanFilesButton_clicked();
        void on_fileSelectSlider_sliderReleased();
        void on_renderDebugCirclesCheckbox_toggled(bool checked);
        void on_saveGreenCirclesButton_clicked();
        void on_renderGreenCirclesCheckbox_toggled(bool checked);
       

};

#endif // UGIP_H
