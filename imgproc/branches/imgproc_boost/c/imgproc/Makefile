########################################
# makefile - Microgravity Imgproc      #
# file: $project/makefile              #
#                                      #
# author: jim pelton                   #
########################################

SVN_V:=-D'SVNVER="$(shell svnversion -n .)"'

CC=g++
INCDIR=-Iboost
CFLAGS=-g -Wall -fPIC -shared  $(INCDIR)

LIBS=-lboost_thread 
LFLAGS=$(LIBDIR) $(LIBS)

OBJS:=$(patsubst %.cpp,%.o,$(wildcard *.cpp))

all: imgproc

imgproc: $(OBJS)
	$(CC) $(CFLAGS) -o $@ $? $(LFLAGS)


clean: 
	/bin/rm -f *.o
	
















