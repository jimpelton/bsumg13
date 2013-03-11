__author__ = 'jim'

import csv



class ugDataWriter:

    df=None

    def __init__(self, dataFile):
        df=dataFile

        return


    def writeValues(self, fileName, valuesList):
        """
        Write a list of values to fileName
        :param fileName: file to write to
        :param valuesList: the values to write.
        """
        print('Writing values: {0}'.format(fileName))
        f = open(fileName, 'w')

        for i in range(len(valuesList)):
            f.write('{0} '.format(str(i)))
            f.write(' '.join(str(x) for x in valuesList[i]))
            f.write('\n')
        f.close()

    def writeGravity(self, filename, gravList):
        print('Writing gravity values: {}'.format(filename))
        f = open(filename, 'w')

        for i in range(len(gravList)):
            f.write('{0} {1}'.format(str(i), gravList[i]))
            # f.write(' '.join(str(x) for x in gravList[i]))
            f.write('\n')
        f.close()

    def writeCSVGravity(self,filename,gravList):
        print('Writing {} gravity values: {}'.format(len(gravList), filename))
        with open(filename,'w') as filewriter:
            cf=csv.writer(filewriter,delimiter=' ')
            for row in gravList:
                cf.writerow(row)

    def writeCSVValues(self, filename, valuesList):
        print('Writing {} values: {}'.format(len(valuesList), filename))
        with open(filename,'w') as filewriter:
            cf=csv.writer(filewriter, deslimiter=' ')
            for row in valuesList:
                cf.writerow(row)


