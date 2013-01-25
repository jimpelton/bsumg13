#include "InputFrame.h"

#include <QtCore/QDebug>
#include <QtCore/QStringList>
#include <QtCore/QDir>
#include <QtCore/QDirIterator>
#include <QtWidgets/QFileDialog>
#include <QtWidgets/QMessageBox>

InputFrame::InputFrame(QWidget *parent)
    : QFrame(parent)
    , m_imageDir()
    , m_outputDir()
    , m_haveValidImageDir(false)
{
    setupUi(this);    
}

InputFrame::~InputFrame() { }

void InputFrame::on_imageDirButton_clicked()
{    QFileDialog dialog(this);
    dialog.setFileMode(QFileDialog::Directory);
    dialog.setOption(QFileDialog::ShowDirsOnly);

    QStringList files;
    if (dialog.exec())
        files = dialog.selectedFiles();

    if (files.size() > 0) {
        QDir dir(files.first());
        if (dir.exists()) {
            m_imageDir = dir.absolutePath();
            imageDirLineEdit->setText(m_imageDir);
            scanFilesDirctoryButton->setEnabled(true);
            m_haveValidImageDir=true;
            emit haveValidImageDir(m_imageDir);
        } else {
            scanFilesDirctoryButton->setEnabled(false);
            m_haveValidImageDir=false;
            QMessageBox::information(NULL, "Invalid directory.", "The directory doesn't exist.");
        }
    }
}

void InputFrame::on_outputDirButton_clicked()
{
    QFileDialog dialog(this);
    dialog.setFileMode(QFileDialog::Directory);
    dialog.setOption(QFileDialog::ShowDirsOnly);

    QStringList files;
    if (dialog.exec())
        files = dialog.selectedFiles();

    if (files.size()>0){
        QDir dir(files.first());
        if (dir.exists()){
            m_outputDir = dir.absolutePath();
            outputDirLineEdit->setText(m_outputDir);
            m_haveValidOutpuDir=true;
            emit haveValidOutputDir(m_outputDir);
        } else {
            m_haveValidOutpuDir=false;
            QMessageBox::information(NULL, "Invalid directory.", "The directory doesn't exist.");
        }
    }
}

void InputFrame::on_scanFilesDirectoryButton_clicked()
{
    QDir dir(m_imageDir);
    if (!dir.exists()) { 
        QMessageBox::information(NULL, "Invalid image directory.", "The image directory doesn't exist.");
        return;
    }

    uint numfiles = dir.count();
    QDirIterator iter(dir.absolutePath(), QStringList() << "*.raw", QDir::Files);
    int found=0;
    while (iter.hasNext()) {
        iter.next();
        ++found;
        emit scannedFileFound(iter.filePath(), found/(float)numfiles);
    }
    qDebug() << "Scanned: " << found << ".raw files.";
}

void InputFrame::on_circlesFileButton_clicked()
{
    QFileDialog dialog(this);
    dialog.setFileMode(QFileDialog::ExistingFile);
    
    QStringList file;
    if (dialog.exec())
        file = dialog.selectedFiles();

    if (file.size() > 0){
        m_circlesFileName = file.front();
        circlesFileLineEdit->setText(m_circlesFileName);
        emit haveCirclesFileName(m_circlesFileName);
    }
}