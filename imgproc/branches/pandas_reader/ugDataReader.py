__author__ = 'jim'

import re
import os
import io

import numpy as np

from math import sqrt
from pandas import DataFrame
from pandas import DatetimeIndex
import pandas

from sys import maxsize


class ugDataReader:
    """
    Reads and parses the files provided by the given ugDataFile into memory.

    :TODO: 2012 file list filtering
    """

    def __init__(self, format_year=2013, num_wells=192, datafile=None):
        """
        :param format_year: year of data, either 2012 or 2013.
        :param num_wells: number of wells to expect.
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
        self._imgFileRegEx_2013 = \
            re.compile("^DataCamera(405|485)_[0-9]{8}_[0-9]{18}.raw.txt$")

    def update(self):
        """
        Update this data reader and read the files provided by the
        ugDataFile object.
        Will update the DataFile object if necessary.
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
        :return: DataFrame or None
        """
        return self._valuesDict.get(typeString)

    def valueTimes(self, typeString):
        """
        return list of time strings for typeString or None if
        typeString is not a valid data type
        """
        return self._valuesDict.get(typeString).index

    def startTimeMillis(self):
        """
        Get the smallest time found in the data set.
        :return: int
        """
        return self._startMillis

    def timeStringDeltaFromStart(self, millis):
        """
        Get the difference millis-startTimeMillis() as a time stamp
        formatted as "Seconds.Milliseconds"
        :param millis: time in millis
        :type millis: int
        :return: string
        """
        delta = (int(millis) - self._startMillis)
        ss = (delta // 1000)
        ff = (delta % 1000)
        return str(ss) + "." + str(ff)

    def _isFile(self, strings):
        """
        Get files from list of strings that match fNameReg regular expression.
        :param strings:
        :return:
        """
        return [x for x in strings if self._imgFileRegEx_2013.match(x) is not None]

    def _makeWellColumnTitles(self):
        alpha = [chr(c) for c in range(65, 73)]  # 'A' through 'H'
        cols = []
        lr = ["L", "R"]

        for i in range(1, 13):
            for w in range(0, 2):
                p = lr[w]
                for c in alpha:
                    cols.append(p + str(c) + str(i))
        return cols

    def _createDateTimeIndex(self, timeList, name="TIME"):
        dti = pandas.DatetimeIndex(data=timeList, name=name, tz="UTC")
        dti.name = name
        return dti

    def _updateStartTime(self, timeDict: dict):
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
            "phid": self._readPhidFiles_2013,
            "baro": self._readBaroFiles_2013,
            "spat": self._readSpatFiles_2013,
            "ni": self._readNIFiles_2013,
            "ups": self._readUpsFiles_2013
        }

        fd = dataFile.filesDict()
        dataTypes = fd.keys()
        for t in dataTypes:
            reader[t](fd[t][0], fd[t][1])  # (basedir, files_list)

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

    def _readBaroFiles_2013(self, basedir, baro_list):
        pass

    def _readNIFiles_2013(self, basedir, ni_list):
        pass

    def _readUpsFiles_2013(self, basedir, ups_list):
        pass

    ##############################################################################
    #   Phidgets Data
    ##############################################################################

    def _readPhidFiles_2013(self, basedir, phid_list):
        print("Reading phidgets DAQ files...")

        def __parseLines(lines, times, data):
            for line in lines[1:]:
                if line == "\n":
                    continue
                line = line.strip()
                lineparts = [int(p) for p in line.split(' ') if not p == "False"]
                times.append(lineparts[0] * 1000000)
                data.append(lineparts[1:])

        time_list = []
        data_list = []
        for f in phid_list:
            with io.open(os.path.join(basedir, f)) as file:
                lines = file.readlines()
                __parseLines(lines, time_list, data_list)

        col = ["HBLK Temp", "HBLK Ambi", "HBLK UV", "ES1 UV", "ES2 UV", "Pres Diff", "N/A", "N/A", "N/A", "N/A"]
        dti = self._createDateTimeIndex(time_list)
        df = DataFrame(data=data_list, index=dti, columns=col)
        self._valuesDict["phid"] = df

        print("Read {} phidgits records.".format(len(df.index)))

    ##############################################################################
    #   Accelerometer Data
    ##############################################################################

    def _readGravFiles_2013(self, basedir, gravityfiles_list):
        """
        Read in gravity files (Accel.txt).
        """
        time_vals = []
        mags = []
        for f in gravityfiles_list:
            with io.open(os.path.join(basedir, f)) as thisfile:
                lines = thisfile.readlines()
                for line in lines[1:]:
                    line = line.strip()
                    txyz = [float(i) for i in line.split(' ')]
                    time_vals.append(int(txyz[0]) * 1000000)
                    gMag = sqrt(txyz[1] * txyz[1] + txyz[2] * txyz[2] + txyz[3] * txyz[3])
                    mags.append([gMag, txyz[1], txyz[2], txyz[3]])

        cols = ["MAG", "X", "Y", "Z"]
        dti = self._createDateTimeIndex(time_vals)
        df = DataFrame(data=mags, index=dti, columns=cols)
        self._valuesDict["grav"] = df
        print('Read {} 2013 gravity files.'.format(len(df.index)))

    def _readSpatFiles_2013(self, basedir, spat_list):
        """
        Read spatial accelerometer files.
        :param basedir:
        :param spat_list:
        :return:
        """
        time_list = []
        mags = []
        for f in spat_list:
            with io.open(os.path.join(basedir, f)) as thisfile:
                lines = thisfile.readlines()
                for line in lines[1:]:
                    line = line.strip()
                    txyz = [float(i) for i in line.split(' ')]
                    time_list.append(int(txyz[0]) * 1000000)
                    gMag = sqrt(txyz[1] * txyz[1] + txyz[2] * txyz[2] + txyz[3] * txyz[3])
                    mags.append([gMag, txyz[1], txyz[2], txyz[3]])

        columns = ['Mag', 'X', 'Y', 'Z']
        dti = self._createDateTimeIndex(time_list)
        df = DataFrame(data=mags, index=dti, columns=columns)
        self._valuesDict["spat"] = df

        print('Read {} Spatial files.'.format(len(df.index)))


    ##############################################################################
    #   Camera Data
    ##############################################################################

    def _read405Files_reversed_2013(self, basedir, files405_list):
        """
        Reads the camera 405 files as in the normal _read405Files_reversed, but
        tries to parse the new file name, which has a millisecond timestamp in it.
        :param files405_list: list of file name strings (relative path names)
        """

        def __parseLines(lines, timeIdx, numwells):
            colIdx = numwells - 1
            for s in lines:
                # s is "wellIdx:wellAvg"
                strs = s.split(':')
                val = int(strs[1].strip())
                vals[timeIdx][colIdx] = val  #add to array backwards
                colIdx -= 1

        files405_list = self._isFile(files405_list)
        vals = np.zeros((len(files405_list), self._numWells), dtype=np.float64)
        time_vals = []
        timeIdx = 0
        for f in files405_list:
            with io.open(os.path.join(basedir, f)) as data:
                lines = data.readlines()
                s = re.split(self._NumReg, f)
                try:
                    t = int(s[5].lstrip("0"))
                    time_vals.append(t * 1000000)  # millisec --> nanosec
                    __parseLines(lines, timeIdx, self._numWells)
                    timeIdx += 1
                except IndexError:
                    print("The filename {} seemed to be invalid, skipping.".format(f))

        cols = self._makeWellColumnTitles()
        dti = self._createDateTimeIndex(time_vals)
        df = DataFrame(data=vals, index=dti, columns=cols)
        self._valuesDict["405"] = df

        print('{} files read.'.format(len(df.index)))

    def _read485Files_2013(self, basedir, files485_list):
        """
        Reads the camera 485 files as in the normal _read485Files, but
        tries to parse the new file name, which has a millisecond timestamp in it.
        :param files485_list:
        """

        def __parseLines(lines, timeIdx):
            colIdx = 0
            for s in lines:
                strs = s.split(':')
                v = int(strs[1].strip())
                vals[timeIdx][colIdx] = v
                colIdx += 1

        files485_list = self._isFile(files485_list)
        vals = np.zeros((len(files485_list), self._numWells), dtype=np.float64)
        time_vals = []
        timeIdx = 0
        for f in files485_list:
            with io.open(os.path.join(basedir, f)) as thisfile:
                lines = thisfile.readlines()
                s = re.split(self._NumReg, f)
                try:
                    t = int(s[5].lstrip("0"))
                    time_vals.append(t * 1000000)  # millisec --> nanosec
                    __parseLines(lines, timeIdx)
                    timeIdx += 1
                except IndexError:
                    print("The filename {} seemed to be invalid, skipping.".format(f))

        cols = self._makeWellColumnTitles()
        dti = self._createDateTimeIndex(time_vals)
        df = DataFrame(data=vals, index=dti, columns=cols)
        self._valuesDict["485"] = df
        print('{} 2013 485 files read.'.format(len(df.index)))

    ##############################################################################
    #   2012 Data
    ##############################################################################

    def _read405Files_reversed(self, basedir, files405_list):
        """
        Read 405 files. Each file contains lines formatted as: "well#:val".
        Note: adds to the row backwards, having a reversing effect, so that the
            well patterns match that of the 485 well values.

        :param dir405: Path to 405 data files (expected to be sorted).
        """

        def __parseLines(lines, timeIdx, numwells):
            colIdx = numwells - 1
            for s in lines:
                # s is "wellIdx:wellAvg"
                strs = s.split(':')
                val = int(strs[1].strip())
                vals[timeIdx][colIdx] = val  #add to array backwards
                colIdx -= 1

        vals = np.zeros((len(files405_list), self._numWells), dtype=np.float64)
        timeIdx = 0
        for f in files405_list:
            with io.open(os.path.join(basedir, f)) as thisfile:
                lines = thisfile.readlines()
                __parseLines(lines, timeIdx, self._numWells)
                timeIdx += 1

        df = DataFrame(data=vals, index=np.arange(timeIdx))
        self._valuesDict["405"] = df

        print('{} files read.'.format(len(df.index)))

    def _read485Files(self, basedir, files485_list):
        """
        Read in them 485 data files.
        Each file contains lines formatted as: "well#:val".
        :param dir485:
        TODO: convert to pandas.
        TODO: add 0.5 increments in Index column
        """

        def __parseLines(lines, timeIdx):
            colIdx = 0
            for s in lines:
                strs = s.split(':')
                v = int(strs[1].strip())
                vals[timeIdx][colIdx] = v
                colIdx += 1

        vals = np.zeros((len(files485_list), self._numWells), dtype=np.float64)
        timeIdx = 0
        for f in files485_list:
            with io.open(os.path.join(basedir, f)) as thisfile:
                lines = thisfile.readlines()
                __parseLines(lines, timeIdx)
                timeIdx += 1

        df = DataFrame(data=vals, index=np.arange(timeIdx))
        self._valuesDict["485"] = df
        print('{} 2012 485 files read.'.format(len(df.index)))


    def _readGravFiles(self, basedir, gravityFiles):
        """
        Read the gravity files and generate magnitudes of the gravity vectors.
        :param gravityFiles: list of files

        TODO: convert to pandas
        TODO: add 0.5 increments to Index offset.
        """
        tempmags = []
        for f in gravityFiles:
            with io.open(os.path.join(basedir, f)) as thisfile:
                line = thisfile.readlines()[0]
                txyz = [float(i) for i in line.split(' ')]
                gMag = sqrt(txyz[0] * txyz[0] + txyz[1] * txyz[1] + txyz[2] * txyz[2])
                tempmags.append([gMag, txyz[0], txyz[1], txyz[2]])

        cols = ["MAG", "X", "Y", "Z"]
        df = DataFrame(data=tempmags, columns=cols)
        self._valuesDict["grav"] = df

        # timeIdx = 0
        # self._valuesDict["grav"] = np.zeros(len(tempmags), dtype=np.float64)
        # accel_array = self._valuesDict["grav"]
        # for m in tempmags:
        #     accel_array[timeIdx] = m
        #     timeIdx += 1

        print('Read {} gravity files.'.format(len(df.index)))

    ##############################################################################
    #   Plate Layout
    ##############################################################################

    # def _readPlateLayout(self):
    #     """
    #     Collect the well plate layout into the "layout" dictionary of this
    #     instance of ugDataReader.
    #
    #     The values in the dictionary are the list of well plate indexes
    #     that each type of well can be found in.
    #     """
    #     if self._dataFile.plateLayout() is None:
    #         return
    #
    #     totalCells = 0
    #     rowCnt = 0
    #     colCnt = 0
    #
    #     with io.open(self._dataFile.plateLayout(), 'r') as csvfile:
    #         reader = csv.reader(csvfile, dialect='excel', delimiter=',')
    #         for row in reader:
    #             rowCnt += 1
    #             for cell in row:
    #                 totalCells += 1
    #                 self._linearLayout.append(cell)
    #
    #     try:
    #         colCnt = int(totalCells / rowCnt)
    #     except ZeroDivisionError as e:
    #         print(e)
    #
    #     # indexes for first row, and first column.
    #     firstRow = self._linearLayout[0:colCnt]
    #
    #     firstCol = []
    #     firstColIdxs = [x for x in range(totalCells)[0:totalCells:colCnt]]
    #     for x in firstColIdxs:
    #         firstCol.append(self._linearLayout[x])
    #
    #     i = 0
    #     for cell in self._linearLayout:
    #         if cell is '':
    #             continue
    #
    #         if cell in firstRow or cell in firstCol:
    #             continue
    #
    #         if cell in self._layout:
    #             self._layout[cell].append(i)
    #         else:
    #             self._layout[cell] = []
    #             self._layout[cell].append(i)
    #         i += 1
    #
    #     return
