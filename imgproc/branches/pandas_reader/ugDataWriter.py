__author__ = 'jim'

import csv
import itertools
import numpy
import ugDataReader

class ugDataWriter:
    def __init__(self, reader=None):
        self._dataReader = reader
        return

    def writeTimeSeries(self, filename, dataArray: numpy.ndarray, timeList: list):
        """
        :param filename:
        :param gravList:
        :param timeList:
        """
        if dataArray is None:
            print("writeTimeSeries failed: dataArray is None.")
            return
        if timeList is None:
            print("writeTimeSeries failed: timeList is None.")
            return
        if filename is None:
            print("writeTimeSeries failed: filename is None.")
            return

        print('Writing values: {}'.format(filename))
        f = open(filename, 'w')
        for t, r in itertools.zip_longest(timeList, dataArray):
            ts = self._dataReader.timeStringDeltaFromStart(t)
            f.write(str(t) + ' ' + ts + ' ')
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
