#include "SaveShowFrame.h"

#include <QtCore/QString>
#include <QtWidgets/QFileDialog>
#include <QtWidgets/QMessageBox>

class QDir;

SaveShowFrame::SaveShowFrame(QWidget *parent)
    : QFrame(parent)
{
    setupUi(this);
}

SaveShowFrame::~SaveShowFrame() { }

void SaveShowFrame::on_saveCirclesButton_clicked()
{
    //QDir::home gives the users home directory. Useful because the file dialog
    //might open the image directory, which might contain thousands of files
    //and that takes a long time!
    QString filename = QFileDialog::getSaveFileName(this,
            "Choose a save location for the circles file.", QDir::homePath());

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
