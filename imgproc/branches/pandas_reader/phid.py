import argparse
import numpy as np
import os



__author__ = 'jim'


def getArgs():
    """
    Get command line arguments.
    :return: list of arguments, or None if user says no.
    """
    parser = argparse.ArgumentParser(description="Calculate calcium concentrations from ratiometric dye intensisties.")
    parser.add_argument("-pd", "--DirectoryPhid", help="Phidgets DAQ files directory")
    parser.add_argument("-dout", "--DirectoryOut", help="Output Directory")


    args = parser.parse_args()

    print("\n ***Check to make sure the following is correct!***\n")
    print(
        "Phidgets Directory:       {}\n"
        "Output Directory:         {}\n"
        .format
        (
            args.DirectoryPhid,
            args.DirectoryOut,
        )
    )

    #Wait for user to check dirs
    yesno = input("Is the above information correct? [y/N]")
    if yesno == 'y' or yesno == 'Y':
        return args
    else:
        return None


def readPhidDir(basedir):
    print ("Scanning phidgets directory for files...")

    values_list = []
    if not os.path.isdir(basedir):
        print("{} appears to not be a directory.".format(basedir))
    else:
        for fname in os.listdir(basedir):
            with open(os.path.join(basedir, fname)) as file:
                lines = file.readlines()
                for l in lines[1:]:
                    if l == "\n":
                        print("Continuing past empty line...")
                        continue
                    l = l.strip().split(' ')
                    vals = [int(n) for n in l if not n == "False"]
                    values_list.append(vals)
    return values_list

# def writePhidValues(filename, values_list):
#     print('Writing values: {}'.format(filename))
#     f = open(filename, 'w')
#     for line in values_list:
#         ts = self._dataReader.timeStringDeltaFromStart(t)
#         f.write(str(t) + ' ' + ts + ' ')
#         f.write(' '.join(str(cell) for cell in r))
#         f.write('\n')
#     f.close()

#
# def _readPhidFiles(self, basedir, phid_list):
#         print("Reading phidgets DAQ files...")
#
#         time_list = self._timesDict["phid"]
#         tempparts = []
#         for f in phid_list:
#             thisfile = open(os.path.join(basedir, f))
#             lines = thisfile.readlines()
#             thisfile.close()
#             #with open(basedir+f) as file:
#             for line in lines:
#                 line.strip()
#                 lineparts = [p for p in line.split(' ') if p is not "False"]
#                 time_list.append(lineparts[0])
#                 tempparts.append(lineparts[1:])
#
#         self._valuesDict["phid"] = np.zeros((len(tempparts), max([len(x) for x in tempparts])), dtype=float)
#         v = self._valuesDict["phid"]
#         idx = 0
#         for line in tempparts:
#             for i in range(len(line)-1):
#                 v[idx][i] = line[i+1]
#                 idx += 1
#
#         print("Read {} phidgits records.".format(idx))




def main():
    args = getArgs()
    if args is None:
        print("bye!")
        return

    phiddir = args.DirectoryPhid
    outdir = args.DirectoryOut
    values_list = readPhidDir(phiddir)

    return




if __name__ == '__main__':
    main()