#########################################################################
# Makefile - Microgravity Imgproc      
# file:      imgproc/c/Makefile              
# date:      03/04/13 
# author:    Jim Pelton
# 2012-2013 Boise State University Microgravity Team
#########################################################################
SVN_V:=-D'SVNVER="$(shell svnversion -n .)"'

CC=g++
INCDIR:=-I/usr/include/boost
CXXFLAGS=-g -Wall -fPIC -shared $(INCDIR)

LIBDIR=
LIBS:=-lboost_thread 
LFLAGS:=$(LIBDIR) $(LIBS)

#OBJS:=$(patsubst %.cpp,%.o,$(wildcard *.cpp))

IMGPROC_LIBS=libimgproc.so 

IMGPROC_OBJS=AbstractImageProcessor.o \
			 CircleDrawingImageProcessor.o \
			 Processor.o \
			 Reader.o \
			 WellIndexImageProcessor.o \
			 Writer.o

all: $(IMGPROC_LIBS)

libimgproc.so: $(IMGPROC_OBJS)
	$(CC) $(CXXFLAGS) -o $@ $(IMGPROC_OBJS) $(LFLAGS)

libimgproc.a: $(IMGPROC_OBJS)
	$(AR)  rcv $@ $(IMGPROC_OBJS)
	ranlib $@

clean: 
	/bin/rm -f *.o
	
















