__author__ = 'jim'

import argparse
import conc
import ugDataReader
import ugDataWriter
import numpy as np
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
    parser.add_argument("-sd", "--DirectorySpat", help="Spatial files directory")
    parser.add_argument("-pd", "--DirectoryPhid", help="Phidgets DAQ files directory")
    parser.add_argument("-nd", "--DirectoryNI", help="NI6008 DAQ files directory")
    parser.add_argument("-ud", "--DirectoryUps", help="Ups files directory")
    parser.add_argument("-dout", "--DirectoryOut", help="Output Directory")
    parser.add_argument("-pl", "--PlateLayout", help="Plate layout file as csv file in excel dialect.")
    parser.add_argument("-s", "--Start", type=int, default=0, help="Starting index.")
    parser.add_argument("-e", "--End", type=int, default=0, help="Ending index (default: all files in directory).")
    parser.add_argument("-nw", "--NumWells", type=int, default=192, choices=(96,192), help="Number of wells that are present in well plate files (max 192) (default: %(default)s).")
    parser.add_argument("-fy", "--FormatYear", type=int, default=2013, choices=(2012,2013), help="Format of the data, depending on uG year (default: %(default)s).")


    args = parser.parse_args()

#    if args.Start is None:
#        args.Start = 0
#
#    if args.End is None:
#        args.End = 0
#
#    if args.NumWells is None:
#        args.NumWells = 192
    

    print("\n ***Check to make sure the following is correct!***\n")
    print(
        "405 Data Files Directory: {}\n"
        "485 Data Files Directory: {}\n"
        "Gravity Directory:        {}\n"
        "Spatial Directory:        {}\n"
        "Phidgets Directory:       {}\n"
        "NI6008 Directory:         {}\n"
        "UPS Directory:            {}\n"
        "Output Directory:         {}\n"
        "Plate Layout:             {}\n"
        "Starting Index:           {}\n"
        "Ending Index:             {}\n"
        "Well Count:               {}\n"
        "Format Year:              {}"
        .format
            (
            args.Directory405, args.Directory485,
            args.DirectoryGrav, args.DirectorySpat,
            args.DirectoryPhid, args.DirectoryNI,
            args.DirectoryUps,
            args.DirectoryOut,
            args.PlateLayout,
            args.Start, args.End, 
            args.NumWells, args.FormatYear
        )
    )

    #Wait for user to check dirs
    yesno = input("Is the above information correct? [y/N]")
    if yesno == 'y' or yesno == 'Y':
        return args
    else:
        return None


#def ccSlice(source):
#    """
#    Returns the co-culture values in a values array arranged as
#    one row of 96 columns
#    :param source: numpy array of 96 well values.
#    :return: one row of 20 columns of the values for the cc wells.
#    """
#    w = range(NUM_WELLS)
#    ccvals = source[w[0:4], w[12:16], w[24:28], w[36:40],
#                    w[48:52], w[60:64], w[72:76]]
#    return ccvals

# def mcSlice(source):
#     """
#     Returns the mc values in source.
#     """
#     w = range(96)
#

#def calculateRatios(values405, values485):
#    """
#    Calculate the ratios of elements in values405 and values485.
#            ratios = values405/values485
#    :param values405: numpy array of 405 values
#    :param values485: numpy array of 485 values
#    :rtype : list
#    :return: An python list of rows of 96 wells for each time in values405 and values485.
#    """
#    print("Calculating Ratios...")
#    shortest = min(len(values405), len(values485))
#    rats = np.zeros((shortest, NUM_WELLS), dtype=np.float64)
#    for row in range(shortest):
#        for col in range(NUM_WELLS):
#            rats[row][col] = values405[row][col] / values485[row][col]
#    return rats


def main():
    args = getArgs()
    if not args:
        exit()

    basedir485 = args.Directory485
    basedir405 = args.Directory405
    gravDir = args.DirectoryGrav
    spatDir = args.DirectorySpat
    phidDir = args.DirectoryPhid
    outDir = args.DirectoryOut
    plateLayout = args.PlateLayout
    start = str(args.Start).zfill(5)
    end = str(args.End).zfill(5)
    numwells = args.NumWells
    formatYear = args.FormatYear

    dataFile = ugDataFile(dir405=basedir405, dir485=basedir485,
                          dirgrav=gravDir, dirspat=spatDir,
                          dirphid=phidDir,
                          dirout=outDir, layout=plateLayout)

    dataFile.fromTo(int(start), int(end))
    dataFile.update()
    dataReader = ugDataReader.ugDataReader(datafile=dataFile, 
                                           num_wells=numwells, 
                                           format_year=formatYear)
    dataReader.update()
    # slice405 = dataReader.valuesList("405")
    # slice485 = dataReader.valuesList("485")
    # gravlist = dataReader.valuesList("grav")


    # ratios = calculateRatios(slice405, slice485)
    # cars = conc.calculateConcentrations(ratios, slice405, slice485)

    # write data files
    dw = ugDataWriter.ugDataWriter(reader=dataReader)
    if basedir405 is not None:
        dw.writeTimeSeries(outDir + '/Data405.dat',
                           dataReader.valuesList("405"),
                           dataReader.valueTimes("405"))

    if basedir485 is not None:
        dw.writeTimeSeries(outDir + '/Data485.dat',
                           dataReader.valuesList("485"),
                           dataReader.valueTimes("485"))

    if gravDir is not None:
        dw.writeTimeSeries(outDir + '/grav.dat',
                           dataReader.valuesList("grav"),
                           dataReader.valueTimes("grav"))

    if spatDir is not None:
        dw.writeTimeSeries(outDir + '/spat.dat',
                           dataReader.valuesList("spat"),
                           dataReader.valueTimes("spat"))

    if phidDir is not None:
        dw.writeTimeSeries(outDir + '/phid.dat',
                           dataReader.valuesList("phid"),
                           dataReader.valueTimes("phid"))


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
