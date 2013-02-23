########################################
# makefile - Microgravity Imgproc      #
# file: $project/makefile              #
#                                      #
# author: jim pelton                   #
########################################

SVN_V:=-D'SVNVER="$(shell svnversion -n .)"'

CC=g++
INCDIR=-Iboost
CFLAGS=-g -Wall -shared -fPIC -O0 -I$(INCDIR)

LIBS=-lboost_filesystem -lboost_program_options
LFLAGS=-L$(LIBDIR) $(LIBS)

OBJS:=$(patsubst %.cpp,%.o,$(wildcard *.cpp))

all: imgproc

imgproc: $(OBJS)
	$(CC) $(CFLAGS) -o $@ $? $(LFLAGS)


clean: 
	/bin/rm -f *.o
	
















