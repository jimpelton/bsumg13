#########################################################################
# Makefile - Microgravity Imgproc      
# file:      c/imgproc/Makefile              
# date:      03/04/13 
# author:    Jim Pelton
# 2012-2013 Boise State University Microgravity Team
#########################################################################
include ../include.mk

SVN_V:=-D'SVNVER="$(shell svnversion -n .)"'

CC=g++
INCDIR:=-I/usr/include/boost
CXXFLAGS=-g -Wall -fPIC -shared $(INCDIR)

LIBDIR=
LIBS:=-lboost_thread-mt
LFLAGS:=$(LIBDIR) $(LIBS)

TESTDIR=Test

#OBJS:=$(patsubst %.cpp,%.o,$(wildcard *.cpp))

IMGPROC_LIBS=libimgproc.so 

IMGPROC_OBJS=AbstractImageProcessor.o \
			 CircleDrawingImageProcessor.o \
			 Processor.o \
			 Reader.o \
			 WellIndexImageProcessor.o \
			 Writer.o \
			 Imgproc.o

IMGPROC_HEADERS=AbstractImageProcessor.h \
				WellIndexImageProcessor.h \
				Processor.h \
				Reader.h \
				Writer.h \
				BufferPool.h \
				Buffer.h \
				ugTypes.h \
				Centers.h \
				Export.h \
				Imgproc.h

IMGPROC_TEST=$(TESTDIR)/ReaderTest.o 


all: $(IMGPROC_LIBS)

libimgproc.so: $(IMGPROC_OBJS)
	$(CC) $(CXXFLAGS) -o $@ $(IMGPROC_OBJS) $(LFLAGS)

libimgproc.a: $(IMGPROC_OBJS)
	$(AR)  rcv $@ $(IMGPROC_OBJS)
	ranlib $@

test: $(IMGPROC_TEST)
	$(CC) $(CXXFLAGS) -o $@ $(IMGPROC_TEST) $(LFLAGS)


install:
	cp $(IMGPROC_HEADERS) ../$(INST_INCDIR)/
	cp $(IMGPROC_LIBS) ../$(INST_LIBDIR)/


clean: 
	/bin/rm -f *.o libimgproc.so
	
















