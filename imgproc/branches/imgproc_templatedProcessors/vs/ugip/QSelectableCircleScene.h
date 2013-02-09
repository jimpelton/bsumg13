#ifndef QCLICKABLEGRAPHICSSCENE_H
#define QCLICKABLEGRAPHICSSCENE_H

#include <QtWidgets/QGraphicsScene>
#include <QtWidgets/QGraphicsItem>
#include <QtWidgets/QGraphicsEllipseItem>
#include <QtWidgets/QGraphicsSceneMouseEvent>

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

    QSelectableEllipse(qreal x, qreal y, qreal r, QGraphicsItem *parent=0) 
        : QGraphicsEllipseItem(x, y, r, r, parent) 
        , m_radius(r/2)
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
    void setRadius(int r) { m_radius = r; }

private:

    Mode m_mode;    

    /// Which color to make the currently selected shape.
    QColor m_color; 

    /// The current graphics item.
    QGraphicsItem *m_item;

    int m_radius;


    void setCircleColor(QColor c);

    void mouseMoveEvent(QGraphicsSceneMouseEvent *event);
    void mousePressEvent(QGraphicsSceneMouseEvent *event);
    void mouseReleaseEvent(QGraphicsSceneMouseEvent *event);

signals:
    void insertedNewCircle(QSelectableEllipse *);
};

#endif // QCLICKABLEGRAPHICSSCENE_H
