#########################################################################
# Makefile - Microgravity Imgproc      
# file:      c/Makefile              
# date:      04/11/13 
# author:    Jim Pelton
# 2012-2013 Boise State University Microgravity Team
#########################################################################

include include.mk

all: imgproc_dir ugip_dir 

imgproc_dir:
	mkdir -p $(INST_INCDIR) $(INST_LIBDIR)
	cd imgproc; make; make install

ugip_dir:
	cd ugip; make

clean:
	cd imgproc; make clean
	cd ugip; make clean

install:
	mkdir -p $(INST_INCDIR) $(INST_LIBDIR)
	cd imgproc; make; make install
	cd ugip; make; make install

