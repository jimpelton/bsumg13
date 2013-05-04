
TEMPLATE = app
TARGET = ugip

INCLUDEPATH +=  ../include/

QT += core gui widgets

CONFIG += qt 
#CONFIG += debug
#CONFIG -= release

LIBS += -L../lib \
        -limgproc \
        -lboost_system \
        -lboost_program_options \
        -lboost_filesystem \
        -lboost_thread

QMAKE_CXXFLAGS += -std=c++11

#CONFIG(unix){
    INCLUDEPATH += /usr/include/boost/ 
             

    CONFIG(debug){
        DESTDIR = build/debug
        OBJECTS_DIR = build/debug/.obj
        MOC_DIR = build/debug/.moc
        UI_DIR = build/debug/.ui
    }

    CONFIG(release){
        message(Release configuration is not yet defined!!!)
    }

#}

FORMS += \
    uigp2.ui \
    SaveShowFrame.ui \
    InputFrame.ui \
    InfoWidget.ui \
    CirclePropsFrame.ui

OTHER_FILES += \
    ugip.pro.user

HEADERS += \
    Util.h \
    uigp2.h \
    SaveShowFrame.h \
    QSelectableCircleScene.h \
    InputFrame.h \
    InfoWidget.h \
#    CLParser.h \
    CirclesFile.h \
    CirclePropsFrame.h

SOURCES += \
    Util.cpp \
    uigp2.cpp \
    SaveShowFrame.cpp \
    QSelectableCircleScene.cpp \
    main.cpp \
    InputFrame.cpp \
    InfoWidget.cpp \
#    CLParser.cpp \
    CirclesFile.cpp
