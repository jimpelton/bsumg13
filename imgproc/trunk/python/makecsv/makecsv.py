__author__ = 'jim'

import argparse
import conc
import ugDataReader
import ugDataWriter
from ugDataFile import ugDataFile



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

def ccSlice(source):
    """
    Returns slice of 405 co-culture values.
    """
    w = range(96)
    ccvals = source[w[0:4], w[12:16], w[24:28], w[36:40],
                            w[48:52], w[60:64], w[72:76]]
    return ccvals

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
    cars = conc.calculateConcentrations(ratios, slice405, slice485)


    
    # write data files
    dw = ugDataWriter.ugDataWriter(dataFile)
    
    dw.writeGravity(dataFile.dirout() + 'grav.dat', dataReader.valuesgrav)
    dw.writeValues(dataFile.dirout() + 'cars.dat', cars.Concs)
    dw.writeValues(dataFile.dirout() + 'F485MaxValues.dat', cars.F485MaxVals)
    dw.writeValues(dataFile.dirout() + 'F405MaxValues.dat', cars.F405MaxVals)
    dw.writeValues(dataFile.dirout() + 'F485MinValues.dat', cars.F485MinVals)
    dw.writeValues(dataFile.dirout() + 'F405MinValues.dat', cars.F405MinVals)
    dw.writeValues(dataFile.dirout() + 'QVals.dat', cars.QVals)
    dw.writeValues(dataFile.dirout() + 'RminVals.dat', cars.RminVals)
    dw.writeValues(dataFile.dirout() + 'RmaxVals.dat', cars.RmaxVals)
    dw.writeValues(dataFile.dirout() + 'NumVals.dat', cars.NumVals)
    dw.writeValues(dataFile.dirout() + 'DenVals.dat', cars.DenVals)


if __name__ == '__main__':
    main()