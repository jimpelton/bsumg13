__author__ = 'jim'

import os, errno
from pylab import *
import numpy as np


def human_sort(self, s):
    return [int(k) if k.isdigit() else k for k in re.split(self._NumReg, s)]


def main(argv):
    """
    :param argv:
    """
    datafile = argv[1]
    gravfile = argv[2]

    plot_title = argv[3]

    plots_dir = os.getcwd() + os.sep + "plots/"

    if not os.path.exists(plots_dir):
        try:
            os.mkdir(plots_dir)
        except OSError as err:
            if err.errno == errno.EEXIST:
                print(plots_dir + " exists already, skipping creation")
            else:
                print(plots_dir + " could not be created.")

    grav = np.loadtxt(gravfile, dtype=np.float)

    startTime = grav[300000, 0]
    endTime = grav[350000, 0]

    grav = grav[300000:350000, :]

    data = np.loadtxt(datafile, dtype=np.float32, usecols=(0, 3))

    sel = [x for x in map(lambda v: startTime < v < endTime, data[:,1])]
    data_sel = [x for x in np.select([sel], [data[:,1]]) if x != 0]

    # t = arange(grav[0, 0], grav[-1, 0], 1, np.float32)

    # fig = figure(figsize=(32, 18), dpi=200)
    # for i in range(1):
    i = 2
    # subplot(211)
    plot(arange(0, len(data_sel)), data_sel, linewidth=1.0)
    xlabel('Time')
    ylabel('Well Brightness Average')
    # subplot(212)
    plot(arange(0,len(grav)), grav[:,1], linewidth=1.0)
    # xlabel('Time')
    # ylabel('Acceleration')
    title(plot_title)
    grid(True)
    savefig(plots_dir + os.path.basename(datafile) + "_" + str(i) + ".png")
    #clf()
    show()


if __name__ == '__main__':
    main(sys.argv)




