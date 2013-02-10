import os
import argparse

from math import sqrt

#######################################################
# getArgs: get CL arguments
#
def getArgs():
    parser = argparse.ArgumentParser()
    parser.add_argument("-d485", "--Directory485", help="485 Data Files Directory")
    parser.add_argument("-d405", "--Directory405", help="405 Data Files Directory")
    parser.add_argument("-gd", "--DirectoryGrav", help="Gravity files directory")
    parser.add_argument("-dout", "--DirectoryOut", help="Output Directory")


    args = parser.parse_args()

    print(
        "405 Data Files Directory {} \n 485 Data Files Directory {} \n"
         "Gravity Directory {} \n Output Directory {} \n"
        .format(
            args.Directory405, args.Directory485, args.RatiosFile,
            args.WellValues405, args.WellValues485, args.DirectoryGrav,
            args.DirectoryOut
        )
    )

    return args



def read405Files(dir405):
    """

    :param dir405:
    :rtype : list
    """
    print('Reading 405 files...'),
    files405_list = [f for f in os.listdir(dir405)]
    values405_list = []
    count = 0
    for f in files405_list:
        thisfile = open(dir405 + f)
        lines = thisfile.readlines()
        thisfile.close()
        values405_list.append([])

        for s in lines:
            strs = s.split(':')
            val = int(strs[1].strip())
            values405_list[count].append(val)

        #reverse to match the 485 well order
        values405_list[count].reverse()
        count += 1

    print('{}'.format(count))
    return values405_list


def read485Files(dir485):
    """


    :rtype : list
    :param dir485:
    :return:
    """
    print('Reading 485 files...'),
    files485_list = [f for f in os.listdir(dir485)]
    values485_list = []
    count = 0
    for f in files485_list:
        thisfile = open(dir485 + f)
        lines = thisfile.readlines()
        thisfile.close()
        values485_list.append([])

        for s in lines:
            strs = s.split(':')
            val = int(strs[1].strip())
            values485_list[count].append(val)

        count += 1

    print('{}'.format(count))
    return values485_list


def readGravityFiles(gravDir):
    """


    :rtype : list
    :param gravDir:
    :return:
    """
    print('Reading and calculating gravity vectors...'),
    gravityFiles = [f for f in os.listdir(gravDir)]
    gravity_list = []
    for f in gravityFiles:
        thisfile = open(gravDir + f)
        line = thisfile.readlines()[0]
        thisfile.close()

        xyzt = [float(i) for i in line.split(' ')]
        gMag = sqrt(xyzt[0] * xyzt[0] + xyzt[1] * xyzt[1] + xyzt[2] * xyzt[2])
        gravity_list.append(gMag)

    print('{}'.format(len(gravity_list)))
    return gravity_list


def calculateRatios(smaller, values405, values485):
    """

    :rtype : list
    :param smaller:
    :param values405:
    :param values485:
    :return:
    """
    ratios_list = []
    for sampNum in range(smaller):
        samples405 = values405[sampNum]
        samples485 = values485[sampNum]
        if not (len(samples405) == 96 and len(samples485) == 96):
            print('dadnamit, not both have 96 wells en im!')
            exit()
        ratios_list.append([])
        for wellNum in range(96):
            r = samples405[wellNum] / samples485[wellNum]
            ratios_list[sampNum].append(r)
    return ratios_list


def calculateRatioAveragesPerWell(ratios):
    """

    :rtype : list
    :param ratios: list of ratios
    :return: list of ratio averages across all time
    """
    ratioAvgs_list = []
    for wellIdx in range(len(ratios)):
        wellSum = 0
        for smpl in range(len(ratios[wellIdx])):
            wellSum = wellSum + ratios[wellIdx][smpl]

        wellSum = wellSum / len(ratios[wellIdx])
        ratioAvgs_list.append(wellSum)
    return ratioAvgs_list

def calculateConcentrations(ratios, val405, val485):
    """

    :rtype : list
    :param ratios: the well ratios on t=[0..tmax]
    :param val405: 405 well values on t=[0..tmax]
    :param val485: 485 well values on t=[0..tmax]
    """
    egtaWells = [72,73,74,75]
    ionoWells = [60,61,62,63]
    Kd = 0.23
    Rmin = min( )
    concs = []


def writeValues(fileName, valuesList):
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


if __name__ == '__main__':
    args = getArgs()
    basedir485 = args.Directory485
    basedir405 = args.Directory405
    gravDir = args.DirectoryGrav

    wv485Name = 'wv485.dat'
    wv405Name = 'wv405.dat'
    ratName = 'rat.dat'
    gravName = 'grav.dat'

    #list of 96-element lists
    gravities = readGravityFiles(gravDir)
    values405 = read405Files(basedir405)
    values485 = read485Files(basedir485)

    if len(values485) < len(values405):
        smaller = len(values485)
        if len(gravities) < smaller:
            smaller = len(gravities)
    else:
        smaller = len(values405)
        if len(gravities) < smaller:
            smaller = len(gravities)

    ratios = calculateRatios(smaller, values405, values485)

    concs = calculateConcentrations(ratios, values405, values485)

    writeValues(wv405Name, values405)
    writeValues(wv485Name, values485)
    writeValues(ratName, ratios)
    writeValues(gravName, gravities)






    #######################################################
    #  write405Values
    #
    # def write405Values(values405_filename, values405_list, gravity_list):
    #     print('Writing 405 values to {0}'.format(values405_filename))
    #     values405_file = open(values405_filename, 'w')
    #     for i in range(len(values405_list)):
    #         row = values405_list[i]
    #         values405_file.write("{0} ".format(str(i)))
    #         values405_file.write("{0} ".format(str(gravity_list[i])))
    #         values405_file.write(' '.join(str(x) for x in values405_list[i]))
    #         values405_file.write('\n')
    #     values405_file.close()

    #######################################################
    #  write485Values
    #
    # def write485Values(values485_filename, values485_list, gravity_list):
    #     print('Writing 485 values to {0}'.format(values485_filename))
    #     values485_file = open(values485_filename, 'w')
    #     for i in range(len(values485_list)):
    #         values485_file.write("{0} ".format(str(i)))
    #         values485_file.write("{0} ".format(str(gravity_list[i])))
    #         values485_file.write(' '.join(str(x) for x in values485_list[i]))
    #         values485_file.write('\n')
    #     values485_file.close()

    #######################################################
    #  writeRatioValues
    #
    # def writeRatioValues(ratios_filename, ratios_list, gravity_list):
    #     print('Writing ratio values to {0}'.format(ratios_filename))
    #     ratios_file = open(ratios_filename, 'w')
    #     for i in range(len(ratios_list)):
    #         ratios_file.write("{0} ".format(str(i)))
    #         if gravity_list != None:
    #             ratios_file.write("{0} ".format(str(gravity_list[i])))
    #         ratios_file.write(' '.join(str(x) for x in ratios_list[i]))
    #         ratios_file.write('\n')
    #     ratios_file.close()
