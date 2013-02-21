import os
import argparse
from math import sqrt


def getArgs():
    """
    Get command line arguments.
    :return: list of arguments, or None if user says no.
    """
    parser = argparse.ArgumentParser(description="Calculate calcium concentrations from ratiometric dye intensisties.")
    parser.add_argument("-d485", "--Directory485", help="485 Data Files Directory")
    parser.add_argument("-d405", "--Directory405", help="405 Data Files Directory")
    parser.add_argument("-gd", "--DirectoryGrav", help="Gravity files directory")
    parser.add_argument("-dout", "--DirectoryOut", help="Output Directory")
    parser.add_argument("-s", "--Start", type=str, help="Starting index")
    parser.add_argument("-e", "--End", type=str, help="Ending index")

    args = parser.parse_args()

    if args.Start is None:
        args.Start = 0

    if args.End is None:
        args.End = 0

    print ("\n ***Check to make sure the following is correct!***\n")
    print(
        "405 Data Files Directory: {}\n"
        "485 Data Files Directory: {}\n"
        "Gravity Directory:        {}\n"
        "Output Directory:         {}\n"
        "Starting Index:           {}\n"
        "Ending Index:             {}"
        .format
        (
            args.Directory405, args.Directory485,
            args.DirectoryGrav, args.DirectoryOut,
            args.Start, args.End
        )
    )

    #Wait for user to check dirs
    yesno = input("Is the above information correct? [y/N]")
    if yesno == 'y':
        return args
    else:
        return None



def read405Files(dir405):
    """
    Read 405 files. Each file contains lines formated as: "well#:val".
    :param dir405: Path to 405 data files
    :rtype : list
    """
    print('Reading 405 files...'),
    files405_list = [f for f in os.listdir(dir405)]
    files405_list.sort()
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
    files485_list.sort()
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
    print('Reading and calculating gravity vectors...')
    gravityFiles = [f for f in os.listdir(gravDir)]
    gravityFiles.sort()
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


def calculateRatios(values405, values485):
    """
    :param start405:
    :rtype : list
    :param smaller:
    :param values405:
    :param values485:
    :return:
    """
    shortest = min(len(values405), len(values485))
    ratios_list = []
    for sampNum in range(shortest):
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

    :param ratios: the well ratios on t=[0..tmax]
    :param val405: 405 well values on t=[0..tmax]
    :param val485: 485 well values on t=[0..tmax]
    """
    shortest  = min( len(ratios), len(val405), len(val485) )
    egtaWells = [ x[72:76] for x in val405 ]
    ionoWells = [ x[60:64] for x in val485 ]
    Kd        = 0.23
    concs = []
    for i in range(shortest):
        F405min = min(egtaWells[i])
        F405max = max(egtaWells[i])

        F485min = min(ionoWells[i])
        F485max = max(ionoWells[i])

        Q = F485min / F485max
        Rmin = F405min/F485min
        Rmax = F405max/F485max
        Ca2_t = []
        for r in ratios[i]:
            Ca2_t.append(Kd * Q * ((r-Rmin)/(Rmax-r)))

        concs.append(Ca2_t)

    return concs


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

def sanitize(args):
    """
    Check the user given directories for existy-ness.
    :param args: The args from a argparser
    :return: true if all input checks out, false otherwise.
    """
    rval=True

    try:
        args.Directory485 = os.path.normpath(args.Directory485)+os.sep
        args.Directory405 = os.path.normpath(args.Directory405)+os.sep
        args.DirectoryGrav = os.path.normpath(args.DirectoryGrav)+os.sep
        args.DirectoryOut = os.path.normpath(args.DirectoryOut)+os.sep
    except:
        return False

    if not os.path.isdir(args.Directory485):
        rval=False
        print ("{} is not a directory (given for Directory485).", args.Directory485)
    if not os.path.isdir(args.Directory405):
        rval=False
        print ("{} is not a directory (given for Directory405).", args.Directory405)
    if not os.path.isdir(args.DirectoryGrav):
        rval=False
        print ("{} is not a directory (given for DirectoryGrav).", args.DirectoryGrav)
    if not os.path.isdir(args.DirectoryOut):
        rval=False
        print ("{} is not a directory (given for DirectoryOut).", args.DirectoryOut)

    return rval



def main():
    args = getArgs()
    if not args:
        exit()

    if not sanitize(args):
        exit()

    basedir485 = args.Directory485
    basedir405 = args.Directory405
    gravDir = args.DirectoryGrav
    outDir = args.DirectoryOut
    start = str(args.Start).zfill(5)
    end = str(args.End).zfill(5)

    wv485Name = outDir+'wv485.dat'
    wv405Name = outDir+'wv405.dat'
    ratName = outDir+'rat.dat'
    gravName = outDir+'grav.dat'
    concName = outDir+'conc.dat'

    #long list of 96-element lists
    gravities = readGravityFiles(gravDir)
    values405 = read405Files(basedir405)
    values485 = read485Files(basedir485)

    gSt=os.listdir(gravDir).index('DataPacket'+start+'.txt')
    st405=os.listdir(basedir405).index('DataCamera405nm'+start+'.raw.txt')
    st485=os.listdir(basedir485).index('DataCamera485nm'+start+'.raw.txt')

    end=int(end)
    start=int(start)
    if (end<=start):
        end = min(len(gravities), len(values405), len(values485))

    nFiles = end-start

    slice405=values405[st405:st405+nFiles]
    slice485=values485[st485:st485+nFiles]
    slicegrav=gravities[gSt:gSt+nFiles]

    ratios = calculateRatios(slice405, slice485)
    concs = calculateConcentrations(ratios, slice405, slice485)

    writeValues(wv405Name, slice405)
    writeValues(wv485Name, slice485)
    writeValues(ratName, ratios)
    writeValues(concName, concs)
    writeGravity(gravName, slicegrav)


if __name__ == '__main__':
    main()


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
