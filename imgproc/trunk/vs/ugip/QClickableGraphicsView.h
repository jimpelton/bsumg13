#ifndef QCLICKABLEGRAPHICSVIEW_H
#define QCLICKABLEGRAPHICSVIEW_H

#include <QtGui/QMouseEvent>
#include <QtWidgets/QGraphicsView>
#include <QtWidgets/QGraphicsScene>
#include <QtWidgets/QGraphicsItemGroup>
#include <QtWidgets/QGraphicsTextItem>
#include <QtWidgets/QGraphicsEllipseItem>

#include <math.h>


class QClickableGraphicsView : public QGraphicsView
{
    Q_OBJECT

public:
    explicit QClickableGraphicsView(QWidget *parent = NULL);

    ~QClickableGraphicsView();
    
    void setScene(QGraphicsScene *scene);

    QList<QGraphicsItem*> getCircles()
    {
        return m_circlesGroup->childItems();
    }

public slots:
    //void mouseReleaseEvent(QMouseEvent *event);

    //void mouseMoveEvent(QMouseEvent *event);

    //void paintEvent(QPaintEvent *);

private:

    QGraphicsTextItem *m_locText;

    QGraphicsScene *m_scene;
    
    QGraphicsItemGroup *m_circlesGroup;

    int m_radius;
    
signals:
    void addedNewCircle(float x, float y, float r);
};

#endif // QCLICKABLEGRAPHICSVIEW_H
