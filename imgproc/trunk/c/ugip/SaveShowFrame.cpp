#include "SaveShowFrame.h"

#include <QtCore/QString>
#include <QtWidgets/QFileDialog>
#include <QtWidgets/QMessageBox>

SaveShowFrame::SaveShowFrame(QWidget *parent)
    : QFrame(parent)
{
    setupUi(this);
}

SaveShowFrame::~SaveShowFrame() { }

void SaveShowFrame::on_saveCirclesButton_clicked()
{
    QString filename = QFileDialog::getSaveFileName(this, 
        "Choose a save location for the circles file.");

    emit saveCircles(filename); 
}

void SaveShowFrame::on_saveImageButton_clicked()
{
    QMessageBox::information(NULL, "No.", "Not implemented.");
}

void SaveShowFrame::on_showDebugCirclesCheckbox_toggled( bool tog )
{
    emit showDebugCircles(tog);
}

void SaveShowFrame::on_showGreenCirclesCheckbox_toggled( bool tog )
{
    emit showPickedCircles(tog);
}
