__author__ = 'jim'

import csv
import itertools
import numpy

class ugDataWriter():
    def __init__(self):
        return

    def writeTimeSeries(self, filename, dataArray: numpy.ndarray, timeList):
        """
        :param filename:
        :param gravList:
        :param timeList:
        """
        print('Writing values: {}'.format(filename))
        f = open(filename, 'w')
        for t, r in itertools.zip_longest(timeList, dataArray):
            f.write(t + ' ')
            f.write(' '.join(str(cell) for cell in r))
            f.write('\n')
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
            cf = csv.writer(filewriter, delimiter=' ')
            for row in valuesList:
                cf.writerow(row)


    #def writeValues(self, fileName, valuesList):
        #     """
        #     Write a list of values to fileName
        #     :param fileName: file to write to
        #     :param valuesList: the values to write.
        #     """
        #     print('Writing values: {0}'.format(self.df.dirout()))
        #     f = open(fileName, 'w')
        #
        #     for i in range(len(valuesList)):
        #         f.write(' '.join(str(x) for x in valuesList[i]))
        #         f.write('\n')
        #     f.close()
