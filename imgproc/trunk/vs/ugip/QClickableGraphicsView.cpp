#include "QClickableGraphicsView.h"
#include "Centers.h"

#include <QDebug>
#include <QtCore/QStringBuilder>
#include <QtCore/QList>
#include <QtWidgets/QGraphicsRectItem>


QClickableGraphicsView::QClickableGraphicsView(QWidget *parent)
    : QGraphicsView(parent)
{

    m_locText = new QGraphicsTextItem();
    m_locText->setPos(mapToScene(10, 10));
    m_locText->setPlainText("Aieeeee!");
    m_locText->setDefaultTextColor(Qt::magenta);
    m_locText->setFlag(QGraphicsItem::ItemIgnoresTransformations);

}

QClickableGraphicsView::~QClickableGraphicsView() { }

void QClickableGraphicsView::setScene(QGraphicsScene *scene)
{
    m_scene=scene;
    //m_locText->setPos(mapToScene(10,10));
    //m_scene->addItem(m_locText);
    QGraphicsView::setScene(scene);
}

//override
//void QClickableGraphicsView::mouseReleaseEvent( QMouseEvent *event )
//{
//    //map viewport coords to scene coords.
//    QPointF pf = mapToScene(event->pos());
//
//    qDebug()<< "Mouse pos: " << pf.x() << ", " << pf.y();
//
//    QList<QGraphicsItem*> items = this->scene()->items(pf);
//    
//    qDebug() << items.size();
//
//    if (items.size() > 0)
//    {
//        QGraphicsItem *circle = items.front(); //TODO: eek! possibly there could be an item infront of the circles.
//        circle->setPos(pf.x()-RADIUS, pf.y()-RADIUS);
//        
//    } 
//    else 
//    {
//        QGraphicsEllipseItem *circle = //new QGraphicsEllipseItem(0,0,RADIUS*2.0,RADIUS*2.0);
//            m_scene->addEllipse
//            (
//                0,0,
//                RADIUS*2.0, RADIUS*2.0, 
//                QPen(Qt::green)
//            );
//        circle->setPos(pf.x()-RADIUS, pf.y()-RADIUS);
//        circle->setZValue(100);
//        circle->setPen(QPen(Qt::green));
//        circle->setFlag(QGraphicsItem::ItemIsMovable);
//                
//        emit addedNewCircle(pf.x(), pf.y(), RADIUS);
//    }
//}

//override
//void QClickableGraphicsView::mouseMoveEvent( QMouseEvent *event )
//{
//    QPointF pf = mapToScene(event->pos());
//    
//    //use QString's internal java-like StringBuilder (%)
//    QString s = QString::number(pf.x()) % ", " % QString::number(pf.y());
//
//    m_locText->setPlainText(s);
//}

//override
//void QClickableGraphicsView::paintEvent( QPaintEvent *event )
//{
//    m_locText->setPos(mapToScene(10,10));
//    QGraphicsView::paintEvent(event);
//}




