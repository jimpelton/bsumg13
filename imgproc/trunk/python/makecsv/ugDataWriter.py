__author__ = 'jim'



class ugDataWriter:

    def __init__(self):
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
        print('Writing gravity values: {}'.format(filename))



