__author__ = 'jim'

import sys
import numpy as np


def usage():
    print("remove_grav.py <grav-file> <data405-file> <data485-file>")


def main(argv):
    if (len(argv) < 2):
        usage()
        return

    gravname = argv[1]
    dataname = argv[2]
    outname = argv[3]
    # data485name = argv[3]

    gravs = np.loadtxt(gravname, dtype=float, usecols=(0, 1))
    times = np.loadtxt(dataname, dtype=int, usecols=(0,))

    grav_avgs = np.zeros((len(times), 2), dtype=float)
    windowsize = 3

    i = 0
    for t in times:
        Gt = gravs[0:-1, 0]
        idx = np.argmin(np.abs(Gt - t))

        wstart = idx - windowsize
        wend = idx + windowsize
        if wstart < 0:
            wstart = 0
        if wend > len(Gt):
            wend = len(Gt)

        Gg = gravs[wstart:wend, 1]
        avg = np.average(Gg)
        grav_avgs[i, 0] = t
        grav_avgs[i, 1] = avg
        i += 1

    np.savetxt(outname, grav_avgs, delimiter=' ', fmt="%d %.8f")

    return

if __name__ == '__main__':
    main(sys.argv)