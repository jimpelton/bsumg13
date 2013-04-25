#include "QSelectableCircleScene.h"

//#include <QDebug>

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

void QSelectableCircleScene::keyPressEvent( QKeyEvent *event )
{
	int key = event->key();
	if (key == Qt::Key_Delete || key == Qt::Key_Backspace) {
		removeSelected();		
	}

	QGraphicsScene::keyPressEvent(event);
}

void QSelectableCircleScene::removeSelected()
{
	QList<QGraphicsItem*> selItems = selectedItems();

	QList<QGraphicsItem*>::const_iterator it = selItems.cbegin();
	QList<QGraphicsItem*>::const_iterator itend = selItems.cend();
	for (; it != itend; ++it) {
		removeItem(*it);
		emit removedExistingCircle(*it);	
	}
}

void QSelectableCircleScene::setRadius( int r )
{
	m_radius = r;
	//foreach (QGraphicsItem *item, items())
	//{
	//	QGraphicsEllipseItem *eee = 
	//		qgraphicsitem_cast<QGraphicsEllipseItem*>(item);
	//	if (eee){
	//		qDebug() << "setting radius...";
	//		QRectF rectangle = eee->rect();
	//		rectangle.setWidth(2*r);
	//		rectangle.setHeight(2*r);
	//		eee->setRect(rectangle);
	//	}
	//}
}




