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
    # if  < 4:
    #     parser.print_usage()
    #     exit(0)

    print(
        "405 Data Files Directory {} \n 485 Data Files Directory {} \n"
         "Gravity Directory {} \n Output Directory {} \n"
        .format
        (
            args.Directory405, args.Directory485,
            args.DirectoryGrav, args.DirectoryOut
        )
    )

    return args



def read405Files(dir405):
    """
    Read 405 files. Each file contains lines formated as: "well#:val".
    :param dir405: Path to 405 data files
    :rtype : list
    """
    print('Reading 405 files...'),
    files405_list = [f for f in os.listdir(dir405)]
    values405_list = []
    timeIdx = 0
    for f in files405_list:
        thisfile = open(dir405 + f)
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


def read485Files(dir485):
    """
    Read in them 485 data files.
    Each file contains lines formatted as: "well#:val".
    :rtype : list
    :param dir485:
    :return: list of lists of wells for each file.
    """
    print('Reading 485 files...'),
    files485_list = [f for f in os.listdir(dir485)]
    values485_list = []
    timeIdx = 0
    for f in files485_list:
        thisfile = open(dir485 + f)
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




def calculateConcentrations(ratios, val405, val485):
    """
    Calculate Ca+2 concentrations from calibration equation:
                 (R-Rmin)
      (Kd * Q) * --------
                 (Rmax-R)
    :rtype : list
    :param ratios: the well ratios on t=[0..tmax]
    :param val405: 405 well values on t=[0..tmax]
    :param val485: 485 well values on t=[0..tmax]
    """
    shortest  = min( len(ratios), len(val405), len(val485) )
    egtaWells = [ x[72:76] for x in val405 ]
    ionoWells = [ x[60:64] for x in val485 ]
    Kd        = 0.23

    for i in range(shortest):
        F405min = min(egtaWells[i][:])
        F485min = max(ionoWells[i][:])
        R = ratios[i]
        Q = min(ionoWells[i])/max(ionoWells[i])



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

def writeGravity(filename, gravList):
    print('Writing gravity values: {}'.format(filename))
    f = open(filename, 'w')

    for i in range(len(gravList)):
        f.write('{0} {1}'.format(str(i), gravList[i]))
        # f.write(' '.join(str(x) for x in gravList[i]))
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

    smaller = min(len(values485), len(values405), len(gravities))

    ratios = calculateRatios(smaller, values405, values485)

    concs = calculateConcentrations(ratios, values405, values485)

    writeValues(wv405Name, values405)
    writeValues(wv485Name, values485)
    writeValues(ratName, ratios)
    writeGravity(gravName, gravities)



# def calculateRatioAveragesPerWell(ratios):
#     """
#     :rtype : list
#     :param ratios: list of ratios
#     :return: list of ratio averages across all time
#     """
#     ratioAvgs_list = []
#     for wellIdx in range(len(ratios)):
#         wellSum = 0
#         for smpl in range(len(ratios[wellIdx])):
#             wellSum = wellSum + ratios[wellIdx][smpl]
#
#         wellSum = wellSum / len(ratios[wellIdx])
#         ratioAvgs_list.append(wellSum)
#     return ratioAvgs_list


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
