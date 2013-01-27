#ifndef INPUTFRAME_H
#define INPUTFRAME_H

#include <QFrame>
#include <QString>

#include "ui_InputFrame.h"

class InputFrame : public QFrame, public Ui::InputFrame
{
    Q_OBJECT

public:
    InputFrame(QWidget *parent);
    ~InputFrame();

    QString getImageDir() const {
        return m_imageDir;
    }

    QString getOutputDir() const {
        return m_outputDir;
    }

    QString getCirclesFileName() const {
        return m_circlesFileName;
    }

signals:
    void imageDir(QString dir);
    void outputDir(QString outDir);
    void circlesFileName(QString circlesFileName);
    void startProcessing();
    void scannedFile(QString filePath, float percent);
    void doneScan();

private:
    QString m_imageDir;
    QString m_outputDir;
    QString m_circlesFileName;

    bool m_haveValidImageDir;
    bool m_haveValidOutpuDir;
    bool m_haveValidCirclesFile;

private slots:
    void on_imageDirButton_clicked();
    void on_outputDirButton_clicked();
    void on_scanFilesDirectoryButton_clicked();
    void on_circlesFileButton_clicked();
    void on_beginProcessingButton_clicked();
};

#endif // INPUTFRAME_H
