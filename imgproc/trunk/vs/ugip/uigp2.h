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
    void addSelectedCircle(QSelectableEllipse* eee);
    void setImageDir(QString dir);
    void setOutputDir(QString dir);
    void setCirclesFileName(QString fname);
    void addScannedFile(QString fname, float percentDone);
    void startProcessing();
    void saveCirclesFile(QString);
    void openCirclesFile();
    void doneScan();
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

    QImage m_currentImage;

    bool is405;

};

#endif // UIGP2_H
