__author__ = 'jim'

import sys
import numpy as np

def usage():
    print("remove_grav.py <data-file> <timedata-file> <output-file>")
    print("<data-file>: 3 columns: millis difference data")
    print("<timedata-file> 2+ columns: millis difference")


def main(argv):
    if (len(argv) < 2):
        usage()
        return

    data_name = argv[1]
    timedata_name = argv[2]
    outname = argv[3]

    # data to truncate
    data = np.loadtxt(data_name, dtype=float)  # time, time offset, values...
    # times to truncate around
    timedata = np.loadtxt(timedata_name, dtype=float, usecols=(0, 1))   # time, time offset [, values...]

    avgs = doavgs(timedata, data)

    fstr="%d %.3f"
    for i in range(avgs.shape[1]-2):
        fstr += " %.8f"

    np.savetxt(outname, avgs, delimiter=' ', fmt=fstr)


def doavgs(timedata, data):
    WINDOW_SIZE = 3
    timerows = timedata.shape[0]
    datacols = data.shape[1]
    avgs = np.zeros((timerows, datacols), dtype=float)

    # get time column from data
    from itertools import zip_longest as zipl
    for t, r  in zipl(timedata, range(timerows)):
        # find index of closest matching time
        Dt = data[0:-1, 0]
        idx = np.argmin(np.abs(Dt - t[0]))
        wstart = idx - WINDOW_SIZE
        wend = idx + WINDOW_SIZE
        if wstart < 0:
            wstart = 0
        if wend > len(Dt):
            wend = len(Dt)-1

        rows = data[wstart:wend, 2:]
        cols = np.transpose(rows)

        avgs[r, 0] = t[0]  # time millis
        avgs[r, 1] = t[1]  # elapsed seconds
        for col, ci in zipl(cols, range(2, datacols-1)):
            avg = np.average(col)
            avgs[r, ci] = avg

    return avgs

if __name__ == '__main__':
    main(sys.argv)
