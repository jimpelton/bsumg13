

#ifndef CirclePropsFrame_h__
#define CirclePropsFrame_h__

#include <QtWidgets/QFrame>
#include <QtWidgets/QWidget>

#include "ui_CirclePropsFrame.h"

class CirclePropsFrame : public QFrame, public Ui::CirclePropsFrame
{
	Q_OBJECT
public:
	CirclePropsFrame(QWidget *parent=0) : QFrame(parent) 
	{
		setupUi(this);
	}

	~CirclePropsFrame(void){}

public slots:
	void on_horizontalSlider_valueChanged( int v )
	{
		emit circleRadiusChanged(v);
	}

signals:
	void circleRadiusChanged(int r);


};

#endif