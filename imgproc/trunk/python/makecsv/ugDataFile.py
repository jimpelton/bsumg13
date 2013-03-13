from asynchat import find_prefix_at_end

__author__ = 'jim'

import os
import re


class ugDataFile:

    def __init__(self, dir405, dir485, dirgrav, dirout):
        self.__dir405 = dir405
        self.__dir485 = dir485
        self.__dirgrav = dirgrav
        self.__dirout = dirout
        self.__files405 = []
        self.__files485 = []
        self.__filesgrav = []
        self.__startingIndex = 0
        self.__endingIndex = 0
        self.__shortest = 0
        self.__needsUpdate = True

    def update(self):
        if self.__needsUpdate:
            if not self.__sanitize():
                raise Exception("Bad input aieeee!")

            print("DataFile doing update.\n")
            self.__files405 = []
            self.__files485 = []
            self.__filesgrav = []
            self.__readNames()
            self.__shortest = min(self.__files405, self.__files485, self.__filesgrav)
            self.__needsUpdate = False

    def fileNames(self, typeString):
        if typeString == "405":
            return self.__files405
        elif typeString == "485":
            return self.__files485
        elif typeString == "grav":
            return self.__filesgrav
        else:
            return None

    def fromTo(self, sIdx=0, eIdx=0):
        if sIdx > eIdx:
            temp = eIdx
            sIdx = temp
            eIdx = sIdx

        self.__startingIndex = sIdx
        self.__endingIndex = eIdx
        self.__needsUpdate = True

    def __readNames(self):
        self.__filesgrav = os.listdir(self.__dirgrav)
        self.__filesgrav.sort()

        self.__files405 = os.listdir(self.__dir405)
        self.__files405.sort()

        self.__files485 = os.listdir(self.__dir485)
        self.__files485.sort()

        if self.__startingIndex == self.__endingIndex:
            self.__findStartingIndex()
            self.__findEndingIndex()
        else:
            self.__filesgrav = \
                self.__filesgrav[self.__startingIndex:self.__endingIndex]
            self.__files405 = \
                self.__files405[self.__startingIndex:self.__endingIndex]
            self.__files485 = \
                self.__files485[self.__startingIndex:self.__endingIndex]

    def __findStartingIndex(self):
        grex = "^DataPacket([0-9]{5}).txt$"
        reg405 = "^DataCamera405nm([0-9]{5}).raw.txt$"
        reg485 = "^DataCamera485nm([0-9]{5}).raw.txt$"

        gravIdx = idx405 = idx485 = 0
        try:
            m = re.match(grex, self.__filesgrav[0])
            if m:
                gravIdx = m.group(1)

            m = re.match(reg405, self.__files405[0])
            if m:
                idx405 = m.group(1)

            m = re.match(reg485, self.__files485[0])
            if m:
                idx485 = m.group(1)

            self.__startingIndex = int(max(gravIdx, idx405, idx485))

        except Exception as eek:
            print("Exception: ".format(eek))

    def __findEndingIndex(self):
        grex = "^DataPacket([0-9]{5}).txt$"
        reg405 = "^DataCamera405nm([0-9]{5}).raw.txt$"
        reg485 = "^DataCamera485nm([0-9]{5}).raw.txt$"
        maxidx = min(len(self.__filesgrav),
                     len(self.__files405),
                     len(self.__files485)) - 1

        if maxidx < 0:
            raise IndexError("maxidx is too small.")

        gravIdx = idx405 = idx485 = 0
        try:
            m = re.match(grex, self.__filesgrav[maxidx])
            if m:
                gravIdx = m.group(1)

            m = re.match(reg405, self.__files405[maxidx])
            if m:
                idx405 = m.group(1)

            m = re.match(reg485, self.__files485[maxidx])
            if m:
                idx485 = m.group(1)

            self.__endingIndex = int(min(gravIdx, idx405, idx485))

        except Exception as eek:
            print("Exception: {0}".format(eek))

    def __sanitize(self):
        """
        Check the user given directories for existy-ness.
        :param args: The args from a argparser
        :return: true if all input checks out, false otherwise.
        """
        rval = True

        try:
            self.__dir485 = os.path.normpath(self.__dir485) + os.sep
            self.__dir405 = os.path.normpath(self.__dir405) + os.sep
            self.__dirgrav = os.path.normpath(self.__dirgrav) + os.sep
            self.__dirout = os.path.normpath(self.__dirout) + os.sep
        except:
            return False

        if not os.path.isdir(self.__dir485):
            rval = False
            print("{} is not a directory (given for Directory485).", self.__dir485)
        if not os.path.isdir(self.__dir405):
            rval = False
            print("{} is not a directory (given for Directory405).", self.__dir405)
        if not os.path.isdir(self.__dirgrav):
            rval = False
            print("{} is not a directory (given for DirectoryGrav).", self.__dirgrav)
        if not os.path.isdir(self.__dirout):
            rval = False
            print("{} is not a directory (given for DirectoryOut).", self.__dirout)

        return rval



    def sIdx(self):
        return self.__startingIndex

    def eIdx(self):
        return self.__endingIndex

    def length(self):
        return (self.eIdx() - self.sIdx()) + 1

    def dir405(self):
        return self.__dir405

    def dir485(self):
        return self.__dir485

    def dirgrav(self):
        return self.__dirgrav

    def dirout(self):
        return self.__dirout
