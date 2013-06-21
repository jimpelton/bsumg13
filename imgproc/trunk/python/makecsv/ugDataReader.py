from _dbm import open

__author__ = 'jim'

from math import sqrt
import csv
import re
from datetime import *
from sys import maxsize

import numpy as np


class ugDataReader:
    def __init__(self, format_year=2013, num_wells=192, datafile=None):
        """
        :param datafile: a ugDataFile object.
        """
        self._startMillis = 0
        self._dataFile = datafile
        self._formatYear = format_year
        self._numWells = num_wells

        self._valuesDict = dict()
        self._valuesDict["405"] = None
        self._valuesDict["485"] = None
        self._valuesDict["grav"] = None
        self._valuesDict["spat"] = None
        self._valuesDict["phid"] = None
        self._valuesDict["ni"] = None
        self._valuesDict["ups"] = None

        self._timesDict = dict()
        self._timesDict["405"] = []
        self._timesDict["485"] = []
        self._timesDict["grav"] = []
        self._timesDict["spat"] = []
        self._timesDict["phid"] = []
        self._timesDict["ni"] = []
        self._timesDict["ups"] = []

        self._layout = dict()
        self._linearLayout = []

        self._NumReg = re.compile("([0-9]+)")


    def update(self):
        """
        Update this data reader and read the files provided by the
        ugDataFile object.
        """
        self._dataFile.update()
        print("DataReader doing update.\n")
        self._readall(self._dataFile)
        self._updateStartTime(self._timesDict)


    def layout(self):
        """
        Returns the plate layout as a dictionary.
        :return: dict
        """
        return self._layout

    def linearLayout(self):
        """
        Returns the entire plate layout as a linear list.
        :return: list
        """
        return self._linearLayout

    def valuesList(self, typeString):
        """
        Get the numpy ndarray for the given type (405, 485, grav, spat, etc).
        :param: typeString: str
        :return: ndarray or None
        """
        return self._valuesDict.get(typeString)


    def valueTimes(self, typeString):
        """
        return list of time strings for typeString or None if
        typeString is not a valid data type
        """
        return self._timesDict.get(typeString)

    def startTimeMillis(self):
        return self._startMillis

    def timeStringDeltaFromStart(self, millis):
        delta = (millis - self._startMillis)
        ss = (delta // 1000)
        ff = (delta % 1000)
        return str(ss) + "." + str(ff)

    def _updateStartTime(self, timeDict):
        lastmin = maxsize
        for timeList in timeDict.values():
            if not len(timeList) is 0:
                lastmin = min(lastmin, min(timeList))
        self._startMillis = lastmin

    def _readall(self, dataFile):
        reader = {
            "405": self._choose405FormatYear,
            "485": self._choose485FormatYear,
            "grav": self._chooseAccelFormatYear,
            "phid": self._readPhidFiles,
            "baro": self._readBaroFiles,
            "spat": self._readSpatFiles,
            "ni": self._readNIFiles,
            "ups": self._readUpsFiles
        }

        fd = dataFile.filesDict()
        dataTypes = fd.keys()
        for t in dataTypes:
            reader[t](fd[t][0], fd[t][1])  # (basedir, files_list)

        self._readPlateLayout()


    def _choose405FormatYear(self, basedir, files405_list):
        print('Reading {0} format 405 files...'.format(self._formatYear))

        if self._formatYear == 2013:
            self._read405Files_reversed_2013(basedir, files405_list)
        else:
            self._read405Files_reversed(basedir, files405_list)

    def _choose485FormatYear(self, basedir, files485_list):
        print('Reading {0} format 485 files...'.format(self._formatYear))

        if self._formatYear == 2013:
            self._read485Files_2013(basedir, files485_list)
        else:
            self._read485Files(basedir, files485_list)

    def _chooseAccelFormatYear(self, basedir, gravity_list):
        print('Reading {0} format gravity files'.format(self._formatYear))

        if self._formatYear == 2013:
            self._readGravFiles_2013(basedir, gravity_list)
        else:
            self._readGravFiles(basedir, gravity_list)


    def _readBaroFiles(self, basedir, baro_list):
        pass

    def _readPhidFiles(self, basedir, phid_list):
        print("Reading phidgets DAQ files...")

        time_list = self._timesDict["phid"]
        tempparts = []
        for f in phid_list:
            with open(basedir+f) as file:
                for line in file.readlines():
                    line.strip()
                    lineparts = [p for p in line.split(' ') if p is not "False"]
                    time_list.append(lineparts[0])
                    tempparts.append(lineparts[1:])

        self._valuesDict["phid"] = np.zeros((len(tempparts), max([len(x) for x in tempparts])), dtype=float)
        v = self._valuesDict["phid"]
        idx = 0
        for line in tempparts:
            for i in range(len(line)-1):
                v[idx][i] = line[i+1]
                idx += 1

        print("Read {} phidgits records.".format(idx))


    def _readSpatFiles(self, basedir, spat_list):
        print("Reading spatial files...")
        tempmags = []
        time_list = self._timesDict["spat"]

        for f in spat_list:
            thisfile = open(basedir + f)
            lines = thisfile.readlines()
            thisfile.close()
            for line in lines[1:]:
                line = line.strip()
                txyz = [float(i) for i in line.split(' ')]
                gMag = sqrt(txyz[1] * txyz[1] + txyz[2] * txyz[2] + txyz[3] * txyz[3])
                tempmags.append(gMag)
                time_list.append(int(txyz[0]))

        timeIdx = 0
        self._valuesDict["spat"] = np.zeros((len(tempmags), 1), dtype=np.float64)
        spat_array = self._valuesDict["spat"]
        for m in tempmags:
            spat_array[timeIdx][0] = m
            timeIdx += 1

        print('Read {} Spatial files.'.format(timeIdx))

    def _readNIFiles(self, basedir, ni_list):
        pass

    def _readUpsFiles(self, basedir, ups_list):
        pass

    def _read405Files_reversed_2013(self, basedir, files405_list):
        """
        Reads the camera 405 files as in the normal _read405Files_reversed, but
        tries to parse the new file name, which has a millisecond timestamp in it.
        :param files405_list:
        """
        self._valuesDict["405"] = np.zeros((len(files405_list), self._numWells), dtype=np.float64)
        vals = self._valuesDict["405"]
        time_vals = self._timesDict["405"]
        timeIdx = 0
        for f in files405_list:
            thisfile = open(basedir + f)
            lines = thisfile.readlines()
            thisfile.close()

            s = re.split(self._NumReg, f)
            t = int(s[5].lstrip("0"))
            time_vals.append(t)

            colIdx = self._numWells - 1
            for s in lines:
                strs = s.split(':')
                val = int(strs[1].strip())
                vals[timeIdx][colIdx] = val  #add to array backwards
                colIdx -= 1

            timeIdx += 1

        print('{} files read.'.format(timeIdx))


    def _read405Files_reversed(self, basedir, files405_list):
        """
        Read 405 files. Each file contains lines formatted as: "well#:val".
        Note: adds to the row backwards, having a reversing effect, so that the
            well patterns match that of the 485 well values.
        :param dir405: Path to 405 data files (expected to be sorted).
        """
        self._valuesDict["405"] = np.zeros((len(files405_list), self._numWells), dtype=np.float64)
        vals = self._valuesDict["405"]
        timeIdx = 0
        for f in files405_list:
            thisfile = open(basedir + f)
            lines = thisfile.readlines()
            thisfile.close()

            colIdx = self._numWells - 1
            for s in lines:
                strs = s.split(':')
                val = int(strs[1].strip())
                vals[timeIdx][colIdx] = val  # add to list backwards
                colIdx -= 1

            timeIdx += 1

        print('{} files read.'.format(timeIdx))


    def _read485Files_2013(self, basedir, files485_list):
        """
        Reads the camera 485 files as in the normal _read485Files, but
        tries to parse the new file name, which has a millisecond timestamp in it.
        :param files485_list:
        """
        self._valuesDict["485"] = np.zeros((len(files485_list), self._numWells), dtype=np.float64)
        vals = self._valuesDict["485"]
        time_vals = self._timesDict["485"]
        timeIdx = 0
        for f in files485_list:
            thisfile = open(basedir + f)
            lines = thisfile.readlines()
            thisfile.close()

            s = re.split(self._NumReg, f)
            t = int(s[5].lstrip("0"))
            time_vals.append(t)

            colIdx = 0
            for s in lines:
                strs = s.split(':')
                v = int(strs[1].strip())
                vals[timeIdx][colIdx] = v
                colIdx += 1
            timeIdx += 1

        print('{} files read.'.format(timeIdx))


    def _read485Files(self, basedir, files485_list):
        """
        Read in them 485 data files.
        Each file contains lines formatted as: "well#:val".
        :param dir485:
        """
        self._valuesDict["485"] = np.zeros((len(files485_list), self._numWells), dtype=np.float64)
        vals = self._valuesDict["485"]
        timeIdx = 0
        for f in files485_list:
            thisfile = open(basedir + f)
            lines = thisfile.readlines()
            thisfile.close()
            colIdx = 0
            for s in lines:
                strs = s.split(':')
                v = int(strs[1].strip())
                vals[timeIdx][colIdx] = v
                colIdx += 1
            timeIdx += 1

        print('{}'.format(timeIdx))

    def _readGravFiles_2013(self, basedir, gravityfiles_list):
        time_list = self._timesDict["grav"]

        mags = []
        for f in gravityfiles_list:
            thisfile = open(basedir + f)
            lines = thisfile.readlines()
            thisfile.close()
            for line in lines[1:]:
                line = line.strip()

                txyz = [float(i) for i in line.split(' ')]

                time_list.append(int(txyz[0]))

                gMag = sqrt(txyz[1] * txyz[1] + txyz[2] * txyz[2] + txyz[3] * txyz[3])
                mags.append(gMag)

        timeIdx = 0
        self._valuesDict["grav"] = np.zeros((len(mags), 1), dtype=np.float64)
        accel_array = self._valuesDict["grav"]
        for m in mags:
            accel_array[timeIdx][0] = m
            timeIdx += 1

        print('Read {} gravity files.'.format(timeIdx))

    def _readGravFiles(self, basedir, gravityFiles):
        """
        Read the gravity files and generate magnitudes of the gravity vectors.
        :param gravityFiles: list of files
        """
        timeIdx = 0
        tempmags = []
        for f in gravityFiles:
            # if timeIdx >= self.valuesaccel.shape[0]:
            #     break

            with open(basedir + f) as thisfile:
                line = thisfile.readlines()[0]
                thisfile.close()
                txyz = [float(i) for i in line.split(' ')]
                gMag = sqrt(txyz[0] * txyz[0] + txyz[1] * txyz[1] + txyz[2] * txyz[2])
                tempmags.append(gMag)
                timeIdx += 1

        timeIdx = 0
        self._valuesDict["grav"] = np.zeros(len(tempmags), dtype=np.float64)
        accel_array = self._valuesDict["grav"]
        for m in tempmags:
            accel_array[timeIdx] = m
            timeIdx += 1

        print('Read {} gravity files.'.format(timeIdx))


    def _readPlateLayout(self):
        """
        Collect the well plate layout into the "layout" dictionary of this
        instance of ugDataReader.

        The values in the dictionary are the list of well plate indexes
        that each type of well can be found in.
        """
        if self._dataFile.plateLayout() is None:
            return

        totalCells = 0
        rowCnt = 0
        colCnt = 0

        with open(self._dataFile.plateLayout(), 'r', newline='') as csvfile:
            reader = csv.reader(csvfile, dialect='excel', delimiter=',')
            for row in reader:
                rowCnt += 1
                for cell in row:
                    totalCells += 1
                    self._linearLayout.append(cell)

        try:
            colCnt = int(totalCells / rowCnt)
        except ZeroDivisionError as e:
            print(e)

        # indexes for first row, and first column.
        firstRow = self._linearLayout[0:colCnt]

        firstCol = []
        firstColIdxs = [x for x in range(totalCells)[0:totalCells:colCnt]]
        for x in firstColIdxs:
            firstCol.append(self._linearLayout[x])

        i = 0
        for cell in self._linearLayout:
            if cell is '':
                continue

            if cell in firstRow or cell in firstCol:
                continue

            if cell in self._layout:
                self._layout[cell].append(i)
            else:
                self._layout[cell] = []
                self._layout[cell].append(i)
            i += 1

        return

    def makeTimeStamp(self, millis):
        delta = (millis - self._startMillis) * 1000  # microseconds
        s = str(datetime.utcfromtimestamp(delta))
        return s




