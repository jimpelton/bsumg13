__author__ = 'jim'

import csv
import ugDataFile


class ugDataWriter():
    def __init__(self, datafile=None):
        self.df = datafile if datafile is not None else None
        return

    def writeValues(self, fileName, valuesList):
        """
        Write a list of values to fileName
        :param fileName: file to write to
        :param valuesList: the values to write.
        """
        print('Writing values: {0}'.format(self.df.dirout()))
        f = open(fileName, 'w')

        for i in range(len(valuesList)):
            f.write(' '.join(str(x) for x in valuesList[i]))
            f.write('\n')
        f.close()

    def writeGravity(self, filename, gravList):
        """

        :param filename:
        :param gravList:
        """
        print('Writing gravity values: {}'.format(filename))
        f = open(filename, 'w')
        f.write('\n'.join(' '.join(str(cell) for cell in row) for row in gravList))
        f.close()

    def writeCSVGravity(self, filename, gravList):
        """

        :param filename:
        :param gravList:
        """
        print('Writing {} gravity values: {}'.format(len(gravList), filename))
        with open(filename, 'w') as filewriter:
            cf = csv.writer(filewriter, delimiter=' ')
            for row in gravList:
                cf.writerow(row)

    def writeCSVValues(self, filename, valuesList):
        """

        :param filename:
        :param valuesList:
        """
        print('Writing {} values: {}'.format(len(valuesList), filename))
        with open(filename, 'w') as filewriter:
            cf = csv.writer(filewriter, deslimiter=' ')
            for row in valuesList:
                cf.writerow(row)


