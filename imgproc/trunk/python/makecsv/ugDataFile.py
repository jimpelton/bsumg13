__author__ = 'jim'

import os
import re
# import csv



class ugDataFile:
    """
    Encapsulates a directory of raw data files which were created by ugip.
    Reads directories and stores a list of the filenames for each file
    in the list.
    """

    def __init__(self,
                 layout=None, dir405=None, dir485=None,
                 dirgrav=None, dirbaro=None, dirphid=None,
                 dirspat=None, dirni=None, outdir=None):

        self._startingIndex = 0
        self._endingIndex = 0
        self._shortest = 0
        self._needsUpdate = True
        self._layout = layout
        self._outdir = outdir

        self._filesDict = dict()

        dir405 = self.sanitize(dir405)
        if dir405 is not None:
            self._filesDict["dir405"] = (dir405, [])

        dir485 = self.sanitize(dir485)
        if dir485 is not None:
            self._filesDict["dir485"] = (dir485, [])

        dirgrav = self.sanitize(dirgrav)
        if dirgrav is not None:
            self._filesDict["dirgrav"] = (dirgrav, [])

        dirbaro = self.sanitize(dirbaro)
        if dirbaro is not None:
            self._filesDict["dirbaro"] = (dirbaro, [])

        dirphid = self.sanitize(dirphid)
        if dirphid is not None:
            self._filesDict["dirphid"] = (dirphid, [])

        dirspat = self.sanitize(dirspat)
        if dirspat is not None:
            self._filesDict["dirspat"] = (dirspat, [])

        dirni = self.sanitize(dirni)
        if dirni is not None:
            self._filesDict["dirni"] = (dirni, [])

        self._NumReg = re.compile("([0-9]+)")

    def sanitize(self, path):
        if path is None:
            return None

        path = os.path.normpath(path) + os.sep
        if os.path.isdir(path):
            return path
        else:
            print(path + " is not a directory!")
            return None

    def update(self):
        if self._needsUpdate:
            print("DataFile doing update.")
            self._readNames()

            lseq = [len(v[1]) for v in self._filesDict.values()]
            if not len(lseq) == 0:
                self._shortest = min(lseq)
            else:
                self._shortest = 0

            if self._startingIndex == 0 and self._endingIndex == 0:
                self._endingIndex = self._shortest

            self._needsUpdate = False

    def fileNames(self, typeString):
        """
        Get the file names for the files for the given value type.
        :param typeString:
        :return: list
        """
        d = self._filesDict.get(typeString)
        if d is not None:
            return d[1][self._startingIndex:self._endingIndex + 1]
        else:
            return []

    def fromTo(self, sIdx=0, eIdx=0):
        """
        Set the starting and ending indexes of files to read.
        :param sIdx:
        :param eIdx:
        """
        if sIdx > eIdx:
            temp = eIdx
            sIdx = temp
            eIdx = sIdx

        self._startingIndex = sIdx
        self._endingIndex = eIdx
        self._needsUpdate = True


    def human_sort(self, s):
        return [int(k) if k.isdigit() else k for k in re.split(self._NumReg, s)]

    def _readNames(self):
        """
        Populate the files dictionary with filenames for each kind of data
        """
        for item in self._filesDict.values():
            if not os.path.isdir(item[0]):
                print("{0} does not appear to be directory.".format(item[0]))
                continue
            for x in os.listdir(item[0]):
                item[1].append(x)
            item[1].sort(key=self.human_sort)

            # if self._startingIndex == self._endingIndex:
            #      self._findStartingIndex()
            #      self._findEndingIndex()
            # else:
            # self._filesgrav = \
            #     self._filesgrav[self._startingIndex:self._endingIndex]
            # self._files405 = \
            #     self._files405[self._startingIndex:self._endingIndex]
            # self._files485 = \
            #     self._files485[self._startingIndex:self._endingIndex]


    def sIdx(self):
        """
        Starting index (the first index).
        :return: start index as an int.
        """
        return self._startingIndex


    def eIdx(self):
        """
        Ending index.
        :return:
        """
        return self._endingIndex


    def length(self):
        """
        Number of elements in the data file.
        :return: int
        """
        return self.eIdx() - self.sIdx()


    def dir405(self):
        """
        405 files directory.
        :return: list of str
        """
        return self._filesDict["dir405"][0]


    def dir485(self):
        """
        485 files directory.
        :return: list of str
        """
        return self._filesDict["dir485"][0]


    def dirgrav(self):
        """
        Gravity files directory.
        :return: list of str
        """
        return self._filesDict["dirgrav"][0]

    def dirspat(self):
        """
        Spatial files directory
        :return: list of str
        """
        return self._filesDict["dirspat"][0]

    def dirout(self):
        """
        Output directory (if saving data).
        :return: str
        """
        return self._outdir


    def plateLayout(self):
        """
        Returns the filename of the plate layout csv file.
        :return: str
        """
        return self._layout


    def filesDict(self):
        return self._filesDict



        # def _findStartingIndex(self):
        #     """
        #     Find the starting index in the directory.
        #     :return: int
        #     """
        #     grex = "^DataPacket([0-9]{5}).txt$"
        #     reg405 = "^DataCamera405nm([0-9]{5}).raw.txt$"
        #     reg485 = "^DataCamera485nm([0-9]{5}).raw.txt$"
        #
        #     gravIdx = idx405 = idx485 = 0
        #     try:
        #         m = re.match(grex, self._filesgrav[0])
        #         if m:
        #             gravIdx = m.group(1)
        #
        #         m = re.match(reg405, self._files405[0])
        #         if m:
        #             idx405 = m.group(1)
        #
        #         m = re.match(reg485, self._files485[0])
        #         if m:
        #             idx485 = m.group(1)
        #
        #         self._startingIndex = int(max(gravIdx, idx405, idx485))
        #
        #     except Exception as eek:
        #         print("Exception: ".format(eek))

        # def _findEndingIndex(self):
        #     """
        #     Find the ending index in the directory.
        #     :return: int
        #     """
        #     grex = "^DataPacket([0-9]{5}).txt$"
        #     reg405 = "^DataCamera405nm([0-9]{5}).raw.txt$"
        #     reg485 = "^DataCamera485nm([0-9]{5}).raw.txt$"
        #
        #
        #     maxidx = min(
        #         len(self._files405),
        #         len(self._files485)) - 1
        #
        #
        #
        #     if maxidx < 0:
        #         raise IndexError("maxidx is too small.")
        #
        #     gravIdx = idx405 = idx485 = 0
        #     try:
        #         m = re.match(grex, self._filesgrav[maxidx])
        #         if m:
        #             gravIdx = m.group(1)
        #
        #         m = re.match(reg405, self._files405[maxidx])
        #         if m:
        #             idx405 = m.group(1)
        #
        #         m = re.match(reg485, self._files485[maxidx])
        #         if m:
        #             idx485 = m.group(1)
        #
        #         self._endingIndex = int(min(gravIdx, idx405, idx485))
        #
        #     except Exception as eek:
        #         print("Exception: {0}".format(eek))
        #
        # def _isSanitary(self):
        #     """
        #     Check the user given directories for existy-ness.
        #     :param args: The args from a argparser
        #     :return: true if all input checks out, false otherwise.
        #     """
        #     rval = True
        #     for item in self._filesDict.items():
        #         path = item[1][0]
        #         path = os.path.normpath(path) + os.sep
        #         if not os.path.isdir(path):
        #             print("{0} is not a directory (given for {1}).", fileTuple[0], item[0])
        #             return False
        #         else:
        #             newtuple = (path, item[1][1])
        #             self._filesDict.items



        # try:
        #     self._dir485 = os.path.normpath(self._dir485) + os.sep
        #
        #     self._dir405 = os.path.normpath(self._dir405) + os.sep
        #
        #
        #     self._dirgrav = os.path.normpath(self._dirgrav) + os.sep
        #
        #     self._dirout = os.path.normpath(self._dirout) + os.sep
        #
        #     if self._layout is not None:
        #         self._layout = os.path.normpath(self._layout)
        #
        # except:
        #     return False

        # if not os.path.isdir(self._dir485):
        #     rval = False
        #     print("{} is not a directory (given for Directory485).", self._dir485)
        #
        # if not os.path.isdir(self._dir405):
        #     rval = False
        #     print("{} is not a directory (given for Directory405).", self._dir405)
        #
        #
        # if not os.path.isdir(self._dirgrav):
        #     rval = False
        #     print("{} is not a directory (given for DirectoryGrav).", self._dirgrav)
        #
        # if not os.path.isdir(self._dirout):
        #     rval = False
        #     print("{} is not a directory (given for DirectoryOut).", self._dirout)
        #
        # if self._layout is not None:
        #     if not os.path.isfile(self._layout):
        #         rval = False
        #         print("{} is not a file (given for PlateLayout).", self._layout)
        #
        # return rval

