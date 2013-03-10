from asynchat import find_prefix_at_end

__author__ = 'jim'

import os
import re

class ugDataFile:

    __dir405  = ""
    __dir485  = ""
    __dirgrav = ""
    __dirout  = ""

    __files405  = None
    __files485  = None
    __filesgrav = None

    __startingIndex=0
    __endingIndex=0

    __needsUpdate=True



    def __init__(self, dir405, dir485, dirgrav, dirout):
        self.__dir405=dir405
        self.__dir485=dir485
        self.__dirgrav=dirgrav
        self.__dirout=dirout

    def update(self):
        if self.__needsUpdate:
            print("DataFile doing update.\n")
            self.__files405=[]
            self.__files485=[]
            self.__filesgrav=[]
            self.__readNames()
            self.__needsUpdate=False

    def __readNames(self):
        self.__filesgrav=os.listdir(self.__dirgrav)
        self.__filesgrav.sort()

        self.__files405=os.listdir(self.__dir405)
        self.__files405.sort()

        self.__files485=os.listdir(self.__dir485)
        self.__files485.sort()

        if self.__startingIndex == self.__endingIndex:
            self.findStartingIndex()
            self.findEndingIndex()
        else:
            self.__filesgrav = \
                self.__filesgrav[self.__startingIndex:self.__endingIndex]
            self.__files405 = \
                self.__files405[self.__startingIndex:self.__endingIndex]
            self.__files485 = \
                self.__files485[self.__startingIndex:self.__endingIndex]




    def findStartingIndex(self):
        grex="^DataPacket([0-9]{5}).txt$"
        reg405="^DataCamera405nm([0-9]{5}).raw.txt$"
        reg485="^DataCamera485nm([0-9]{5}).raw.txt$"

        gravIdx=idx405=idx485=0
        try:
            m = re.match(grex, self.__filesgrav[0])
            if m:
                gravIdx = m.group(1)

            m=re.match(reg405, self.__files405[0])
            if m:
                idx405=m.group(1)

            m=re.match(reg485, self.__files485[0])
            if m:
                idx485=m.group(1)

            self.__startingIndex = int( max(gravIdx, idx405, idx485) )

        except Exception as eek:
            print("Exception: ".format(eek))

    def findEndingIndex(self):
        grex="^DataPacket([0-9]{5}).txt$"
        reg405="^DataCamera405nm([0-9]{5}).raw.txt$"
        reg485="^DataCamera485nm([0-9]{5}).raw.txt$"
        maxidx=min(len(self.__filesgrav), len(self.__files405), len(self.__files485))-1

        if maxidx < 0:
            raise IndexError("maxidx is too small.")

        gravIdx=idx405=idx485=0
        try:
            m=re.match(grex, self.__filesgrav[maxidx])
            if m:
                gravIdx=m.group(1)

            m=re.match(reg405, self.__files405[maxidx])
            if m:
                idx405=m.group(1)

            m=re.match(reg485, self.__files485[maxidx])
            if m:
                idx485=m.group(1)

            self.__endingIndex= int( min(gravIdx, idx405, idx485) )

        except Exception as eek:
            print("Exception: {0}".format(eek))


    def fileNames(self, type):
        if type == "405":
            return self.__files405
        elif type == "485":
            return self.__files485
        elif type == "grav":
            return self.__filesgrav
        else:
            return None

    def fromTo(self, sIdx=0, eIdx=0):
        if sIdx > eIdx:
            temp=eIdx
            sIdx=temp
            eIdx=sIdx

        self.__startingIndex=sIdx
        self.__endingIndex=eIdx
        self.__needsUpdate=True

    def sIdx(self):
        return self.__startingIndex

    def eIdx(self):
        return self.__endingIndex

    def length(self):
        return (self.eIdx() - self.sIdx())+1

    def dir405(self):
        return self.__dir405

    def dir485(self):
        return self.__dir485

    def dirgrav(self):
        return self.__dirgrav

    def dirout(self):
        return self.__dirout
