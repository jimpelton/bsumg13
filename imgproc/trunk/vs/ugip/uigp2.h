#ifndef UIGP2_H
#define UIGP2_H

//#include "InfoWidget.h"
#include "QSelectableCircleScene.h"
#include "CirclesFile.h"

#include <QtWidgets/QMainWindow>
#include <QtWidgets/QGraphicsView>
#include <QtCore/QVector>

#include "ui_uigp2.h"

class SaveShowFrame;
class InputFrame;
class InfoWidget;


class QString;
class QImage;


class uigp2 : public QMainWindow , public Ui::uigp2
{
    Q_OBJECT

public:
    uigp2(QWidget *parent = 0);
    ~uigp2();

public slots:
    /// 
    void addSelectedCircle(QSelectableEllipse* eee);
    
    /// 
    void setImageDir(QString dir);

    /// set the output directory to dir
    void setOutputDir(QString dir);
    
    /// set the circles file to the given filename.
    void setCirclesFileName(QString fname);
    
    /// adds the file name to the list of image files.
    void addScannedFile(QString fname, float percentDone);
    
    /// start processing (un-implemented)
    void startProcessing();
    
    /// save the existing circles to the specified file.
    void saveCirclesFile(QString);

    /// open circles file, replaces existing CirclesFile object.
    void openCirclesFile();
    
    /// display image 0, called after scanning image directory.
    void doneScan();

    /// display image idx in the list of image files.
    void displayImage(int idx);

private:

/************************************************************************/
/* GUI COMPONENTS REFS                                                  */
/************************************************************************/
    QSelectableCircleScene *m_scene;
    SaveShowFrame *m_saveShowFrame;
    InputFrame *m_inputFrame;

/************************************************************************/
/* STUFFS                                                               */
/************************************************************************/
    QString m_imageDir;
    QString m_outputDir;
    QString m_circlesFileName;
    QStringList m_fileNameList;

    QVector<CenterInfo> m_circlesList;
    CirclesFile m_circlesFile;

    QImage m_currentImage;
};

#endif // UIGP2_H
