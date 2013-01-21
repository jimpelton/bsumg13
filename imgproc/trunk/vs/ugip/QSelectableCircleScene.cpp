#include "QSelectableCircleScene.h"

#include "Centers.h"

QSelectableCircleScene::QSelectableCircleScene(QObject *parent)
    : QGraphicsScene(parent)
{
    m_mode = InsertCircle;
    m_color = QColor(Qt::green);
}

QSelectableCircleScene::~QSelectableCircleScene() { }

void QSelectableCircleScene::mouseMoveEvent( QGraphicsSceneMouseEvent *event )
{
    if (event->buttons() == Qt::LeftButton && m_mode == MoveCircle)
    {
        QGraphicsScene::mouseMoveEvent(event);
    }
}

void QSelectableCircleScene::mousePressEvent( QGraphicsSceneMouseEvent *event )
{
    QSelectableEllipse *circle;
    QPointF pf = event->scenePos();

    QList<QGraphicsItem*> clickedItems = items(pf);
    if (clickedItems.size()) {
        m_mode=MoveCircle;
    }

    switch (m_mode)
    {
    case InsertCircle:
        circle = new QSelectableEllipse(0,0,RADIUS*2.0, RADIUS*2.0);
        circle->setPen(QPen(m_color));
        circle->setZValue(100);
        circle->setVisible(true);
        addItem(circle);
        circle->setPos(pf.x()-RADIUS, pf.y()-RADIUS);
        emit insertedNewCircle(circle);
    	break;
    case AdjustCircle:
    case MoveCircle:
    default:
        ;
    }
    QGraphicsScene::mousePressEvent(event);
}

void QSelectableCircleScene::mouseReleaseEvent( QGraphicsSceneMouseEvent *event )
{
    m_mode=InsertCircle;
    QGraphicsScene::mouseReleaseEvent(event);
}





