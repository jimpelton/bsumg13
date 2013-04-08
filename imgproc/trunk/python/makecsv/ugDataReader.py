__author__ = 'jim'

from math import sqrt
import ugDataFile

import numpy as np

NUM_WELLS = 96


class ugDataReader():
    def __init__(self, datafile=None):

        if datafile is None:
            self.df = None
        else:
            self.df = datafile

        dflen = self.df.length()
        self.values405 = np.zeros((dflen, NUM_WELLS), dtype=np.float64)
        self.values485 = np.zeros((dflen, NUM_WELLS), dtype=np.float64)
        self.valuesgrav = np.zeros((dflen, NUM_WELLS), dtype=np.float64)


    def update(self):
        self.df.update()
        print("DataReader doing update.\n")
        self._readall(self.df)

    def _readall(self, df):
        self._read405Files_reversed(df.fileNames("405"))
        self._read485Files(df.fileNames("485"))
        self._readGravityFiles(df.fileNames("grav"))

    def valuesList(self, typeString):
        """
        Get the list for the given type (405, 485, grav).
        :param typeString:
        :return:
        """
        if typeString == "405":
            return self.values405
        elif typeString == "485":
            return self.values485
        elif typeString == "grav":
            return self.valuesgrav
        else:
            return None

    def _read405Files_reversed(self, files405_list):
        """
        Read 405 files. Each file contains lines formated as: "well#:val".
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
                self.values405[timeIdx][colIdx] = val
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

            thisfile = open(basedir + f)
            line = thisfile.readlines()[0]
            thisfile.close()
            xyzt = [float(i) for i in line.split(' ')]
            gMag = sqrt(xyzt[0] * xyzt[0] + xyzt[1] * xyzt[1] + xyzt[2] * xyzt[2])
            self.valuesgrav[timeIdx] = gMag
            timeIdx += 1

        print('{}'.format(timeIdx))



