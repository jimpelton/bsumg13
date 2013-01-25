#ifndef SAVESHOWFRAME_H
#define SAVESHOWFRAME_H

#include <QFrame>

#include "ui_SaveShowFrame.h"
/**
  *	\brief QFrame with Save Image, Save Circles and show checkboxes.
  *	
  *	Emits:
  *	   - showPickedCircles(bool) when the showpickedcircles checkbox is tog'ed.
  *	   - showDebugCircles(bool) when the debugcircles checkbox is tog'ed.
  *	   - saveCircles(QString) with the filename to save as when a filename is chosen
  *	      to save the circles as.
  */
class SaveShowFrame : public QFrame, public Ui::SaveShowFrame
{
    Q_OBJECT

public:
    SaveShowFrame(QWidget *parent=0);
    ~SaveShowFrame();

signals:
    void showPickedCircles(bool);
    void showDebugCircles(bool);
    /// send the filename of the desired circles.
    void saveCircles(QString);

private slots:
    void on_saveCirclesButton_clicked();
    void on_saveImageButton_clicked();
    void on_showDebugCirclesCheckbox_toggled(bool);
    void on_showGreenCirclesCheckbox_toggled(bool);
    

    
};

#endif // SAVESHOWFRAME_H
