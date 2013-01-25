
#ifndef InfoWidget_h__
#define InfoWidget_h__

#include <QtWidgets/QWidget>

#include "ui_InfoWidget.h"


class InfoWidget : public QWidget, public Ui::InfoWidget
{
    Q_OBJECT
public:
    InfoWidget(QWidget *parent=0);
    ~InfoWidget();

};


#endif // InfoWidget_h__

