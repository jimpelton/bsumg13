
import os
import argparse
from ugDataFile import ugDataFile
import ugDataReader
import re

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

    print("\n ***Check to make sure the following is correct!***\n")
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



def getEgta(wellIdx):
    cc = '0[0-3]|1[2-5]|2[4-7]|3[6-9]|4[8-9]|5[0-1]'  #co cul
    mc = '0[4-7]|1[6-9]|2[8-9]|3[0-1]|4[0-3]|5[2-5]'  #mc-3t3
    ml = '0[8-9]|1[0-1]|2[0-3]|3[2-5]|4[4-7]|5[6-9]'  #mlo-y4

    wellIdx = str(wellIdx).zfill(2)
    if re.match(cc, wellIdx):
        return slice(72,76,1)
    elif re.match(mc, wellIdx):
        return slice(76,80,1)
    elif re.match(ml, wellIdx):
        return slice(80,84,1)
    else:
        return None

def getIono(wellIdx):
    cc = '^0[0-3]|1[2-5]|2[4-7]|3[6-9]|4[8-9]|5[0-1]$'  #co cul
    mc = '^0[4-7]|1[6-9]|2[8-9]|3[0-1]|4[0-3]|5[2-5]$'  #mc-3t3
    ml = '^0[8-9]|1[0-1]|2[0-3]|3[2-5]|4[4-7]|5[6-9]$'  #mlo-y4

    wellIdx = str(wellIdx).zfill(2)
    if re.match(cc, wellIdx):
        return slice(60,64,1)
    elif re.match(mc, wellIdx):
        return slice(64,68,1)
    elif re.match(ml, wellIdx):
        return slice(68,72,1)
    else:
        return None

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
    shortest = min(len(ratios), len(val405), len(val485))
    Kd = 0.23  #dissosiation constant for indo-1 dye.
    concs = []
    for time in range(shortest):
        Ca2_t = []
        for r in ratios[time]:
            well=0
            egtaSlice = getEgta(well)
            ionoSlice = getIono(well)
            F405min = min(val405[time][egtaSlice]) 
            F405max = max(val405[time][ionoSlice]) #should be Iono
            F485min = min(val485[time][ionoSlice]) 
            F485max = max(val485[time][egtaSlice]) #should be EGTA
            Q = F485min / F485max
            Rmin = F405min / F485min
            Rmax = F405max / F485max

            Ca2_t.append(Kd * Q * ((r - Rmin) / (Rmax - r)))
            well+=1

        concs.append(Ca2_t)

    return concs




def sanitize(args):
    """
    Check the user given directories for existy-ness.
    :param args: The args from a argparser
    :return: true if all input checks out, false otherwise.
    """
    rval = True

    try:
        args.Directory485 = os.path.normpath(args.Directory485) + os.sep
        args.Directory405 = os.path.normpath(args.Directory405) + os.sep
        args.DirectoryGrav = os.path.normpath(args.DirectoryGrav) + os.sep
        args.DirectoryOut = os.path.normpath(args.DirectoryOut) + os.sep
    except:
        return False

    if not os.path.isdir(args.Directory485):
        rval = False
        print("{} is not a directory (given for Directory485).", args.Directory485)
    if not os.path.isdir(args.Directory405):
        rval = False
        print("{} is not a directory (given for Directory405).", args.Directory405)
    if not os.path.isdir(args.DirectoryGrav):
        rval = False
        print("{} is not a directory (given for DirectoryGrav).", args.DirectoryGrav)
    if not os.path.isdir(args.DirectoryOut):
        rval = False
        print("{} is not a directory (given for DirectoryOut).", args.DirectoryOut)

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


    wv485Name = outDir + 'wv485.dat'
    wv405Name = outDir + 'wv405.dat'
    ratName = outDir + 'rat.dat'
    gravName = outDir + 'grav.dat'
    concName = outDir + 'conc.dat'

    # dataPakStartName = 'DataPacket' + start + '.txt'
    # cam405StartName = 'DataCamera405nm' + start + '.raw.txt'
    # cam485StartName = 'DataCamera485nm' + start + '.raw.txt'

    dataFile = ugDataFile(basedir405,basedir485,gravDir,outDir)
    dataFile.fromTo(int(start), int(end))
    dataFile.update()
    dataReader=ugDataReader.ugDataReader(dataFile)
    dataReader.update()

    slice405 = dataReader.valuesList("405")
    slice485 = dataReader.valuesList("485")
    ratios = calculateRatios(slice405, slice485)
    concs = calculateConcentrations(ratios, slice405, slice485)

    #long list of 96-element lists (one line per data file)
    # gravities = imgproc.readGravityFiles(gravDir)
    # values405 = imgproc.read405Files(basedir405, files405)
    # values485 = imgproc.read485Files(basedir485, files485)

    #make sure end is appropriate.
    # endint     = int(end)
    # startint   = int(start)
    # if endint <= startint:
    #     endint = min(len(gravities), len(values405), len(values485))
    # nFiles = dataFile.length()


    #slice out the values we want to calculate.
    # slice405 = values405[st405:st405+nFiles]
    # slice485 = values485[st485:st485+nFiles]
    # slicegrav = gravities[gSt:gSt+nFiles]



    # writeValues(wv405Name, slice405)
    # writeValues(wv485Name, slice485)
    # writeValues(ratName, ratios)
    # writeValues(concName, concs)
    # writeGravity(gravName, slicegrav)

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
