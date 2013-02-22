########################################
# makefile - Microgravity Imgproc      #
# file: $project/makefile              #
#                                      #
# author: jim pelton                   #
########################################

CC=g++
INCDIR=-Iboost
CFLAGS=-g -Wall -O0 -I$(INCDIR)

LIBS=-lboost_filesystem -lboost_program_options
LFLAGS=-L$(LIBDIR) $(LIBS)

OBJS:=$(patsubst %.cpp,%.o,$(wildcard *.cpp))

all: imgproc

imgproc: $(OBJS)
	$(CC) $(CFLAGS) -o $@ $? $(LFLAGS)

















