#include "QSelectableCircleScene.h"

#include "Centers.h"

using namespace uG;

QSelectableCircleScene::QSelectableCircleScene(QObject *parent)
    : QGraphicsScene(parent)
    , m_radius(45)
    , m_mode(InsertCircle)
    , m_color(Qt::green) { }

QSelectableCircleScene::~QSelectableCircleScene() { }

void QSelectableCircleScene::mouseMoveEvent( QGraphicsSceneMouseEvent *event )
{
    if (event->buttons() == Qt::LeftButton && m_mode == MoveCircle) {
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

    switch (m_mode) {
    case InsertCircle:
        circle = new QSelectableEllipse(0, 0, 2*m_radius, 2*m_radius);
        circle->setPen(QPen(m_color));
        circle->setZValue(100);
        circle->setVisible(true);
        addItem(circle);
        circle->setPos(pf.x() - m_radius, pf.y() - m_radius);
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





