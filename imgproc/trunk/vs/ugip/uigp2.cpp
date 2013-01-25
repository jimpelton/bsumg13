#include "uigp2.h"


#include "InfoWidget.h"

#include <QtWidgets/QGridLayout>
#include <QtWidgets/QSizePolicy>

uigp2::uigp2(QWidget *parent)
    : QMainWindow(parent)
{
    setupUi(this);
    QWidget *w = new QWidget(this);

    QGridLayout *layout = new QGridLayout(this);
    
    m_infoWidget = new InfoWidget(this);
    m_view = new QGraphicsView(this);
    m_scene = new QSelectableCircleScene();
    m_scene->setSceneRect(0,0,2592,1944);
    m_view->setScene(m_scene);

    layout->addWidget(m_infoWidget,0,0,1,1);
    layout->addWidget(m_view,0,1,1,1);

    w->setLayout(layout);
    setCentralWidget(w);

    connect(m_scene, SIGNAL(insertedNewCircle(QSelectableEllipse*)), 
        this, SLOT(circleAdded(QSelectableEllipse*)));
}

uigp2::~uigp2() { }

void uigp2::circleAdded( QSelectableEllipse* eee )
{

}




