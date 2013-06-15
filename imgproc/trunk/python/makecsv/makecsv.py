__author__ = 'jim'

import argparse
import conc
import ugDataReader
import ugDataWriter
import numpy as np
from ugDataFile import ugDataFile

NUM_WELLS = 192


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
    parser.add_argument("-pl", "--PlateLayout", help="Plate layout file as csv file in excel dialect.")
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
        "Plate Layout:             {}\n"
        "Starting Index:           {}\n"
        "Ending Index:             {}"
        .format
            (
            args.Directory405, args.Directory485,
            args.DirectoryGrav, args.DirectoryOut,
            args.PlateLayout,
            args.Start, args.End
        )
    )

    #Wait for user to check dirs
    yesno = input("Is the above information correct? [y/N]")
    if yesno == 'y' or yesno == 'Y':
        return args
    else:
        return None


def ccSlice(source):
    """
    Returns the co-culture values in a values array arranged as
    one row of 96 columns
    :param source: numpy array of 96 well values.
    :return: one row of 20 columns of the values for the cc wells.
    """
    w = range(NUM_WELLS)
    ccvals = source[w[0:4], w[12:16], w[24:28], w[36:40],
                    w[48:52], w[60:64], w[72:76]]
    return ccvals

# def mcSlice(source):
#     """
#     Returns the mc values in source.
#     """
#     w = range(96)
#

def calculateRatios(values405, values485):
    """
    Calculate the ratios of elements in values405 and values485.
            ratios = values405/values485
    :param values405: numpy array of 405 values
    :param values485: numpy array of 485 values
    :rtype : list
    :return: An python list of rows of 96 wells for each time in values405 and values485.
    """
    print("Calculating Ratios...")
    shortest = min(len(values405), len(values485))
    rats = np.zeros((shortest, NUM_WELLS), dtype=np.float64)
    for row in range(shortest):
        for col in range(NUM_WELLS):
            rats[row][col] = values405[row][col] / values485[row][col]
    return rats


def main():
    args = getArgs()
    if not args:
        exit()

    basedir485 = args.Directory485
    basedir405 = args.Directory405
    gravDir = args.DirectoryGrav
    outDir = args.DirectoryOut
    plateLayout = args.PlateLayout
    start = str(args.Start).zfill(5)
    end = str(args.End).zfill(5)

    dataFile = ugDataFile(dir405=basedir405, dir485=basedir485,
                          dirgrav=gravDir,
                          outdir=outDir, layout=plateLayout)

    dataFile.fromTo(int(start), int(end))
    dataFile.update()
    dataReader = ugDataReader.ugDataReader(datafile=dataFile)
    dataReader.update()
    slice405 = dataReader.valuesList("dir405")
    slice485 = dataReader.valuesList("dir485")
    gravlist = dataReader.valuesList("dirgrav")

    # ratios = calculateRatios(slice405, slice485)
    # cars = conc.calculateConcentrations(ratios, slice405, slice485)

    # write data files
    dw = ugDataWriter.ugDataWriter(dataFile)
    if slice405 is not None:
        dw.writeGravity(dataFile.dirout() + 'Data405.dat',
                        slice405,
                        dataReader.valueTimes("405times"))

    if slice485 is not None:
        dw.writeGravity(dataFile.dirout() + 'Data485.dat',
                        slice485,
                        dataReader.valueTimes("485times"))

    if gravlist is not None:
        dw.writeGravity(dataFile.dirout() + 'grav.dat',
                        gravlist,
                        dataReader.valueTimes("gravtimes"))

        # dw.writeGravity(dataFile.dirout() + 'grav.dat', dataReader.valuesgrav)
        # dw.writeValues(dataFile.dirout() + 'cars.dat', cars.Concs)
        # dw.writeValues(dataFile.dirout() + 'F485MaxValues.dat', cars.F485MaxVals)
        # dw.writeValues(dataFile.dirout() + 'F405MaxValues.dat', cars.F405MaxVals)
        # dw.writeValues(dataFile.dirout() + 'F485MinValues.dat', cars.F485MinVals)
        # dw.writeValues(dataFile.dirout() + 'F405MinValues.dat', cars.F405MinVals)
        # dw.writeValues(dataFile.dirout() + 'QVals.dat', cars.QVals)
        # dw.writeValues(dataFile.dirout() + 'RminVals.dat', cars.RminVals)
        # dw.writeValues(dataFile.dirout() + 'RmaxVals.dat', cars.RmaxVals)
        # dw.writeValues(dataFile.dirout() + 'NumVals.dat', cars.NumVals)
        # dw.writeValues(dataFile.dirout() + 'DenVals.dat', cars.DenVals)


if __name__ == '__main__':
    main()