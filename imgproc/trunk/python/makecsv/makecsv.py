__author__ = 'jim'


import argparse
import conc
import ugDataReader
import ugDataWriter
from ugDataFile import ugDataFile
# import numpy as np


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
    print("Calculating Ratios...")
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


def main():
    args = getArgs()
    if not args:
        exit()

    basedir485 = args.Directory485
    basedir405 = args.Directory405
    gravDir = args.DirectoryGrav
    outDir = args.DirectoryOut
    start = str(args.Start).zfill(5)
    end = str(args.End).zfill(5)

    dataFile = ugDataFile(basedir405, basedir485, gravDir, outDir)
    dataFile.fromTo(int(start), int(end))
    dataFile.update()
    dataReader = ugDataReader.ugDataReader(dataFile)
    dataReader.update()

    slice405 = dataReader.valuesList("405")
    slice485 = dataReader.valuesList("485")
    ratios = calculateRatios(slice405, slice485)
    concs = conc.calculateConcentrations(ratios, slice405, slice485)

    # write data files
    dw = ugDataWriter.ugDataWriter(dataFile)

    dw.writeGravity(dataFile.dirout() + 'grav.dat', dataReader.valuesgrav)
    dw.writeValues(dataFile.dirout() + 'conc.dat', concs)
    dw.writeValues(dataFile.dirout() + 'F485MaxValues.dat', conc.F485MaxVals)
    dw.writeValues(dataFile.dirout() + 'F405MaxValues.dat', conc.F405MaxVals)
    dw.writeValues(dataFile.dirout() + 'F485MinValues.dat', conc.F485MinVals)
    dw.writeValues(dataFile.dirout() + 'F405MinValues.dat', conc.F405MinVals)
    dw.writeValues(dataFile.dirout() + 'QVals.dat', conc.QVals)
    dw.writeValues(dataFile.dirout() + 'RminVals.dat', conc.RminVals)
    dw.writeValues(dataFile.dirout() + 'RmaxVals.dat', conc.RmaxVals)
    dw.writeValues(dataFile.dirout() + 'NumVals.dat', conc.NumVals)
    dw.writeValues(dataFile.dirout() + 'DenVals.dat', conc.DenVals)

if __name__ == '__main__':
    main()


    # def sanitize(args):
    #     """
    #     Check the user given directories for existy-ness.
    #     :param args: The args from a argparser
    #     :return: true if all input checks out, false otherwise.
    #     """
    #     rval = True
    #
    #     try:
    #         args.Directory485 = os.path.normpath(args.Directory485) + os.sep
    #         args.Directory405 = os.path.normpath(args.Directory405) + os.sep
    #         args.DirectoryGrav = os.path.normpath(args.DirectoryGrav) + os.sep
    #         args.DirectoryOut = os.path.normpath(args.DirectoryOut) + os.sep
    #     except :
    #         return False
    #
    #     if not os.path.isdir(args.Directory485):
    #         rval = False
    #         print("{} is not a directory (given for Directory485).", args.Directory485)
    #     if not os.path.isdir(args.Directory405):
    #         rval = False
    #         print("{} is not a directory (given for Directory405).", args.Directory405)
    #     if not os.path.isdir(args.DirectoryGrav):
    #         rval = False
    #         print("{} is not a directory (given for DirectoryGrav).", args.DirectoryGrav)
    #     if not os.path.isdir(args.DirectoryOut):
    #         rval = False
    #         print("{} is not a directory (given for DirectoryOut).", args.DirectoryOut)
    #
    #     return rval