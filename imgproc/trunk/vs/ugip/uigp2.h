#ifndef UIGP2_H
#define UIGP2_H

#include "InfoWidget.h"
#include "QSelectableCircleScene.h"

#include <QMainWindow>
#include <QtWidgets/QGraphicsView>

#include "ui_uigp2.h"

class uigp2 : public QMainWindow, public Ui::uigp2
{
    Q_OBJECT

public:
    uigp2(QWidget *parent = 0);
    ~uigp2();

public slots:
    void circleAdded(QSelectableEllipse* eee);

private:
    InfoWidget *m_infoWidget;
    QGraphicsView *m_view;
    QSelectableCircleScene *m_scene;
};

#endif // UIGP2_H
