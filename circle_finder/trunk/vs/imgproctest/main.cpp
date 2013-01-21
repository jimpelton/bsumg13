#include "imgproctest.h"
#include <QApplication>

int main(int argc, char *argv[])
{
    QApplication a(argc, argv);
    imgproctest w;
    w.setargs(argc, argv);
    w.go();
    w.show();
    return a.exec();
}
