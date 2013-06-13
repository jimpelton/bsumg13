__author__ = 'jim'

from math import sqrt
import operator
import numpy as np
import csv


NUM_WELLS = 192


class ugDataReader():
    def __init__(self, datafile=None):
        """
        :param datafile: a ugDataFile object.
        """
        self.df = datafile
        dflen = self.df.length()
        self.values405 = np.zeros((dflen, NUM_WELLS), dtype=np.float64)
        self.values485 = np.zeros((dflen, NUM_WELLS), dtype=np.float64)
        self.valuesaccel = np.zeros((dflen, NUM_WELLS), dtype=np.float64)
        self.valuesspat = np.zeros((dflen, NUM_WELLS), dtype=np.float64)

        self.values405_times = np.zeros((dflen, NUM_WELLS), dtype=np.long)
        self.values485_times = np.zeros((dflen, NUM_WELLS), dtype=np.long)
        self.valuesgrav_times = np.zeros((dflen, NUM_WELLS), dtype=np.long)

        self._layout = dict()
        self._linearLayout = []


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
            return self.values405
        elif typeString == "dir485":
            return self.values485
        elif typeString == "dirgrav":
            return self.valuesgrav
        else:
            return np.zeros((0, 0))

    def _readall(self, df):
        reader = {
            "dir405": self._read405Files_reversed,
            "dir485": self._read485Files,
            "dirgrav": self._readGravityFiles,
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

    def _read405Files_reversed(self, files405_list):
        """
        Read 405 files. Each file contains lines formatted as: "well#:val".
        Note: adds to the row backwards, having a reversing effect, so that the
            well patterns match that of the 485 well values.
        :param dir405: Path to 405 data files (expected to be sorted).
        :rtype : list of list
        """
        print('Reading 405 files...')
        basedir = self.df.dir405()

        timeIdx = 0
        for f in files405_list:
            if timeIdx >= self.values405.shape[0]:
                break

            thisfile = open(basedir + f)
            lines = thisfile.readlines()
            thisfile.close()

            colIdx = NUM_WELLS - 1
            for s in lines:
                strs = s.split(':')
                val = int(strs[1].strip())
                self.values405[timeIdx][colIdx] = val  #add to list backwards
                colIdx -= 1

            timeIdx += 1

        print('{}'.format(timeIdx))

    def _read485Files(self, files485_list):
        """
        Read in them 485 data files.
        Each file contains lines formatted as: "well#:val".
        :rtype : list
        :param dir485:
        :return: list of lists of wells for each file.
        """
        print('Reading 485 files...')
        basedir = self.df.dir485()

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

    def _readGravityFiles(self, gravityFiles):
        """
        Read the gravity files and generate magnitudes of the gravity vectors.
        :rtype : list
        :param gravityFiles: list of files
        :return:
        """
        print('Reading and calculating gravity vectors...')

        basedir = self.df.dirgrav()
        timeIdx = 0
        for f in gravityFiles:
            if timeIdx >= self.valuesgrav.shape[0]:
                break

            with open(basedir+f) as thisfile:
                line = thisfile.readlines()[0]
                thisfile.close()
                xyzt = [float(i) for i in line.split(' ')]
                gMag = sqrt(xyzt[0] * xyzt[0] + xyzt[1] * xyzt[1] + xyzt[2] * xyzt[2])
                self.valuesgrav[timeIdx] = gMag
                timeIdx += 1

        print('{}'.format(timeIdx))


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