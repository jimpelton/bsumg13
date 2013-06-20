__author__ = 'jim'

import sys
import numpy as np


def usage():
    print("remove_grav.py <grav-file> <data405-file> <output-file>")


def main(argv):
    if (len(argv) < 2):
        usage()
        return

    gravname = argv[1]
    dataname = argv[2]
    outname = argv[3]
    # data485name = argv[3]

    gravs = np.loadtxt(gravname, dtype=float, usecols=(0, 2))
    data = np.loadtxt(dataname, dtype=float, usecols=(0, 1))

    grav_avgs = np.zeros((len(data), 3), dtype=float)
    windowsize = 3

    i = 0
    for t in data:
        Gt = gravs[0:-1, 0]
        idx = np.argmin(np.abs(Gt - t[0]))

        wstart = idx - windowsize
        wend = idx + windowsize
        if wstart < 0:
            wstart = 0
        if wend > len(Gt):
            wend = len(Gt)

        Gg = gravs[wstart:wend, 1]
        avg = np.average(Gg)
        grav_avgs[i, 0] = t[0]  # time millis
        grav_avgs[i, 1] = t[1]  # elapsed seconds
        grav_avgs[i, 2] = avg   # grav avg
        i += 1

    np.savetxt(outname, grav_avgs, delimiter=' ', fmt="%d %.3f %.8f")

    return

if __name__ == '__main__':
    main(sys.argv)