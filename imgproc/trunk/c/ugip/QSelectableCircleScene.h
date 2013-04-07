#ifndef QCLICKABLEGRAPHICSSCENE_H
#define QCLICKABLEGRAPHICSSCENE_H

#include <QtWidgets/QGraphicsScene>
#include <QtWidgets/QGraphicsItem>
#include <QtWidgets/QGraphicsEllipseItem>
#include <QtWidgets/QGraphicsSceneMouseEvent>
#include <QtGui/QKeyEvent>
#include <QRectF>

class QSelectableEllipse : public QGraphicsEllipseItem
{
public:
    enum { Type = UserType + 1 };

    explicit QSelectableEllipse(QGraphicsItem *parent=0)
        : QGraphicsEllipseItem(parent) 
        , m_radius(0)
    {
        setFlag(QGraphicsItem::ItemIsMovable, true);
        setFlag(QGraphicsItem::ItemIsSelectable, true);
        setFlag(QGraphicsItem::ItemSendsGeometryChanges, true);
    }

    QSelectableEllipse(qreal x, qreal y, qreal w, qreal h, QGraphicsItem *parent=0) 
        : QGraphicsEllipseItem(x, y, w, h, parent) 
        , m_radius(w/2)
    {
        setFlag(QGraphicsItem::ItemIsMovable, true);
        setFlag(QGraphicsItem::ItemIsSelectable, true);
        setFlag(QGraphicsItem::ItemSendsGeometryChanges, true);
    }

    ~QSelectableEllipse() { }

    int type() const { return Type; }
    int rad() const { return m_radius; }

private:
    int m_radius;
};

/**
  *  	
  */
class QSelectableCircleScene : public QGraphicsScene
{
    Q_OBJECT

public:
    enum Mode { InsertCircle, AdjustCircle, MoveCircle };

    explicit QSelectableCircleScene(QObject *parent=0);
    ~QSelectableCircleScene();

    void setMode(Mode m)  { m_mode = m; }
    Mode getMode() const  { return m_mode; }
    int getRadius() const { return m_radius; }
    void setRadius(int r);

private:

    Mode m_mode;    

    /// Which color to make the currently selected shape.
    QColor m_color; 

    /// The current graphics item.
    QGraphicsItem *m_item;

	/// set radius for next added circle.
    int m_radius;

	/// set color for the next added circle.
    void setCircleColor(QColor c);

	/// helper to keyPressEvent() for delete and backspace keys.
	void removeSelected();

	/************************************************************************/
	/* EVENT OVERRIDES                                                      */
	/************************************************************************/
    void mouseMoveEvent(QGraphicsSceneMouseEvent *event);
    void mousePressEvent(QGraphicsSceneMouseEvent *event);
    void mouseReleaseEvent(QGraphicsSceneMouseEvent *event);
	void keyPressEvent(QKeyEvent *event);

signals:
    void insertedNewCircle(QSelectableEllipse *);
	void removedExistingCircle(QGraphicsItem*);
};

#endif // QCLICKABLEGRAPHICSSCENE_H
