__author__ = 'jim'

from math import sqrt
import numpy as np


class ugDataReader:

    # datafile
    df = None

    values405=None
    values485=None
    valuesgrav=None

    def __init__(self, datafile):
        self.df=datafile
        return

    def update(self):
        self.df.update()
        print("DataReader doing update.\n")
        self.readall(self.df)

    def readall(self,df):
        # len=df.length()
        self.values405 = self.read405Files_reversed(df.fileNames("405"))
        self.values485 = self.read485Files(df.fileNames("485"))
        self.valuesgrav = self.readGravityFiles(df.fileNames("grav"))

    def read405Files_reversed(self, files405_list):
        """
        Read 405 files. Each file contains lines formated as: "well#:val".
        :param dir405: Path to 405 data files (expected to be sorted).
        :rtype : list of list
        """
        print('Reading 405 files...'),
        #files405_list = [f for f in os.listdir(dir405)]
        #files405_list.sort()
        values405_list = []
        timeIdx = 0
        basedir = self.df.dir405()
        for f in files405_list:
            thisfile = open(basedir+f)
            lines = thisfile.readlines()
            thisfile.close()
            values405_list.append([])

            for s in lines:
                strs = s.split(':')
                val = int(strs[1].strip())
                values405_list[timeIdx].append(val)

            #reverse to match the 485 well order
            values405_list[timeIdx].reverse()
            timeIdx += 1

        print('{}'.format(timeIdx))
        return values405_list

    def read485Files(self, files485_list):
        """
        Read in them 485 data files.
        Each file contains lines formatted as: "well#:val".
        :rtype : list
        :param dir485:
        :return: list of lists of wells for each file.
        """
        print('Reading 485 files...'),
        #files485_list = [f for f in os.listdir(dir485)]
        #files485_list.sort()
        values485_list = []
        timeIdx = 0
        basedir = self.df.dir485()
        for f in files485_list:
            thisfile = open(basedir + f)
            lines = thisfile.readlines()
            thisfile.close()
            values485_list.append([])

            for s in lines:
                strs = s.split(':')
                val = int(strs[1].strip())
                values485_list[timeIdx].append(val)

            timeIdx += 1

        print('{}'.format(timeIdx))
        return values485_list

    def readGravityFiles(self, gravityFiles):
        """
        :rtype : list
        :param gravityFiles: list of files
        :return:
        """
        print('Reading and calculating gravity vectors...')
        gravity_list = []
        basedir = self.df.dirgrav()
        for f in gravityFiles:
            thisfile = open(basedir + f)
            line = thisfile.readlines()[0]
            thisfile.close()
            xyzt = [float(i) for i in line.split(' ')]
            gMag = sqrt(xyzt[0] * xyzt[0] + xyzt[1] * xyzt[1] + xyzt[2] * xyzt[2])
            gravity_list.append(gMag)

        print('{}'.format(len(gravity_list)))
        return gravity_list

    def valuesList(self, type):
        if type == "405":
            return self.values405
        elif type == "485":
            return self.values485
        elif type == "grav":
            return self.valuesgrav
        else:
            return None

