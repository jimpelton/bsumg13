__author__ = 'jim'

from math import sqrt
import numpy as np
import csv
import re

NUM_WELLS = 192


class ugDataReader:
    def __init__(self, format_year=2013, datafile=None):
        """
        :param datafile: a ugDataFile object.
        """
        self.df = datafile
        self.format_year = format_year

        self.npValues405 = None
        self.values485 = None
        self.valuesaccel = None  # np.zeros(dflen, dtype=np.float64)
        self.valuesspat = None   # np.zeros(dflen, dtype=np.float64)

        self.values405_times = []
        self.values485_times = []
        self.valuesaccel_times = []
        self.valuesspat_times = []

        self._layout = dict()
        self._linearLayout = []

        self._NumReg = re.compile("([0-9]+)")


    def update(self):
        """
        Update this data reader and read the files provided by the
        ugDataFile object.
        """
        self.df.update()
        print("DataReader doing update.\n")
        self._readall(self.df)


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
        Get the values list for the given type (405, 485, grav).
        :param: typeString:
        :return: narray
        """
        if typeString == "dir405":
            return self.npValues405
        elif typeString == "dir485":
            return self.values485
        elif typeString == "dirgrav":
            return self.valuesaccel
        else:
            return None

    def valueTimes(self, typeString):
        """return list for typeString"""
        if typeString == "gravtimes":
            return self.valuesaccel_times
        elif typeString == "405times":
            return self.values405_times
        elif typeString == "485times":
            return self.values485_times
        else:
            return None

    def _readall(self, df):
        reader = {
            "dir405": self._readCam405,
            "dir485": self._readCam485,
            "dirgrav": self._readAcc,
            "dirphid": self._readPhid,
            "dirbaro": self._readBaro,
            "dirspat": self._readSpat,
            "dirni": self._readNI
        }

        fd = self.df.filesDict()
        for key in fd.keys():
            reader[key](fd[key][1])

        self._readPlateLayout()

    def _readBaro(self, baro_list):
        pass

    def _readPhid(self, phid_list):
        pass

    def _readSpat(self, spat_list):
        pass

    def _readNI(self, ni_list):
        pass

    def _readCam405(self, files405_list):
        basedir = self.df.dir405()
        print('Reading {0} format 405 files...'.format(self.format_year))

        if self.format_year == 2013:
            self._read405Files_reversed_2013(files405_list, basedir)
        else:
            self._read405Files_reversed(files405_list, basedir)

    def _readCam485(self, files485_list):
        basedir = self.df.dir485()
        print('Reading {0} format 485 files...'.format(self.format_year))

        if self.format_year == 2013:
            self._read485Files_2013(files485_list, basedir)
        else:
            self._read485Files(files485_list, basedir)

    def _readAcc(self, gravity_list):
        basedir = self.df.dirgrav()
        print('Reading {0} format gravity files'.format(self.format_year))

        if self.format_year == 2013:
            self._readGrav_2013(gravity_list, basedir)
        else:
            self._readGrav(gravity_list, basedir)

    def _read405Files_reversed_2013(self, files405_list, basedir):
        """
        Reads the camera 405 files as in the normal _read405Files_reversed, but
        tries to parse the new file name, which has a millisecond timestamp in it.
        :param files405_list:
        """
        self.npValues405 = np.zeros((len(files405_list), NUM_WELLS), dtype=np.float64)
        timeIdx = 0
        for f in files405_list:
            if timeIdx >= self.npValues405.shape[0]:
                break

            thisfile = open(basedir + f)
            lines = thisfile.readlines()
            thisfile.close()

            s = re.split(self._NumReg, f)
            self.values405_times.append(s[5])

            colIdx = NUM_WELLS - 1
            for s in lines:
                strs = s.split(':')
                val = int(strs[1].strip())
                self.npValues405[timeIdx][colIdx] = val  #add to list backwards
                colIdx -= 1

            timeIdx += 1

        print('{} files read.'.format(timeIdx))


    def _read405Files_reversed(self, files405_list, basedir):
        """
        Read 405 files. Each file contains lines formatted as: "well#:val".
        Note: adds to the row backwards, having a reversing effect, so that the
            well patterns match that of the 485 well values.
        :param dir405: Path to 405 data files (expected to be sorted).
        """
        self.values405 = np.zeros((len(files405_list), NUM_WELLS), dtype=np.float64)
        timeIdx = 0
        for f in files405_list:
            if timeIdx >= self.npValues405.shape[0]:
                break

            thisfile = open(basedir + f)
            lines = thisfile.readlines()
            thisfile.close()

            colIdx = NUM_WELLS - 1
            for s in lines:
                strs = s.split(':')
                val = int(strs[1].strip())
                self.npValues405[timeIdx][colIdx] = val  #add to list backwards
                colIdx -= 1

            timeIdx += 1

        print('{} files read.'.format(timeIdx))


    def _read485Files_2013(self, files485_list, basedir):
        """
        Reads the camera 485 files as in the normal _read485Files, but
        tries to parse the new file name, which has a millisecond timestamp in it.
        :param files485_list:
        """
        self.values485 = np.zeros((len(files485_list), NUM_WELLS), dtype=np.float64)
        timeIdx = 0
        for f in files485_list:
            if timeIdx >= self.values485.shape[0]:
                break

            thisfile = open(basedir + f)
            lines = thisfile.readlines()
            thisfile.close()

            s = re.split(self._NumReg, f)
            self.values485_times.append(s[5])

            colIdx = 0
            for s in lines:
                strs = s.split(':')
                val = int(strs[1].strip())
                self.values485[timeIdx][colIdx] = val
                colIdx += 1
            timeIdx += 1

        print('{} files read.'.format(timeIdx))


    def _read485Files(self, files485_list, basedir):
        """
        Read in them 485 data files.
        Each file contains lines formatted as: "well#:val".
        :param dir485:
        """
        self.values485 = np.zeros((len(files485_list), NUM_WELLS), dtype=np.float64)
        timeIdx = 0
        for f in files485_list:
            if timeIdx >= self.values485.shape[0]:
                break

            thisfile = open(basedir + f)
            lines = thisfile.readlines()
            thisfile.close()
            colIdx = 0
            for s in lines:
                strs = s.split(':')
                val = int(strs[1].strip())
                self.values485[timeIdx][colIdx] = val
                colIdx += 1
            timeIdx += 1

        print('{}'.format(timeIdx))

    def _readGrav_2013(self, gravityFiles, basedir):
        timeIdx = 0
        tempmags = []
        for f in gravityFiles:
            # if timeIdx >= self.valuesaccel.shape[0]:
            #     break

            thisfile = open(basedir + f)
            lines = thisfile.readlines()
            thisfile.close()
            for line in lines[1:]:
                line = line.strip()
                txyz = [float(i) for i in line.split(' ')]
                gMag = sqrt(txyz[1] * txyz[1] + txyz[2] * txyz[2] + txyz[3] * txyz[3])
                tempmags.append(gMag)
                self.valuesaccel_times.append(str(txyz[0]))
                timeIdx += 1

        timeIdx = 0
        self.valuesaccel = np.zeros((len(tempmags), 1), dtype=np.float64)
        for m in tempmags:
            self.valuesaccel[timeIdx][0] = m
            timeIdx += 1

        print('Read {} gravity files.'.format(timeIdx))

    def _readGrav(self, gravityFiles, basedir):
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
        self.valuesaccel = np.zeros(len(tempmags), dtype=np.float64)
        for m in tempmags:
            self.valuesaccel[timeIdx] = m
            timeIdx += 1

        print('Read {} gravity files.'.format(timeIdx))


    def _readPlateLayout(self):
        """
        Collect the well plate layout into the "layout" dictionary of this
        instance of ugDataReader.

        The values in the dictionary are the list of well plate indexes
        that each type of well can be found in.
        """
        if self.df.plateLayout() is None:
            return

        totalCells = 0
        rowCnt = 0
        colCnt = 0

        with open(self.df.plateLayout(), 'r', newline='') as csvfile:
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

