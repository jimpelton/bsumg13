
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

    SaveShowFrame *getSaveShowFrame() const {
        return this->saveShowFrame;
    }

    InputFrame *getInputFrame() const {
        return this->inputFrame;
    }

};


#endif // InfoWidget_h__

