__author__ = 'jim'

import sys
import os
import numpy as np


def usage():
    print("remove_grav.py <grav-file> <data405-file> <data485-file>")


def main(argv):
    if (len(argv) < 2):
        usage()
        return

    gravname = argv[1]
    dataname = argv[2]
    outname  = argv[3]
    # data485name = argv[3]

    gravs = np.loadtxt(gravname, dtype=float, usecols=(1,))
    times = np.loadtxt(dataname, dtype=int, usecols=(0,))

    grav_avgs = np.zeros((len(times), 2), dtype=float)


    split_grav = np.array_split(gravs, len(times))
    for i in range(len(split_grav)):
        grav_avgs[i,0] = times[i]
        grav_avgs[i,1] = np.average(split_grav[i])

    # callables = { 'float': floatfmt }
    # np.set_printoptions(formatter=callables)
    np.savetxt(outname, grav_avgs, delimiter=" ", fmt="%d, %.8f")

    # gIdx = 0
    # lastt = 0
    # T = np.zeros(1, dtype=np.float)
    # for i in range(len(data_405)):
    #     t1 = data_405[i]
    #     t2 = data_405[i + 1]
    #     tm = (t1 + t2) / 2.0
    #     g = data_grav[gIdx:-1, 1]
    #     condlist = [g > lastt, g < tm]
    #
    #     lastt = tm

    return

def floatfmt(f):
    return str(int(f))

if __name__ == '__main__':
    main(sys.argv)