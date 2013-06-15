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
    file = argv[1]
    plot_title = argv[2]
    plots_dir = os.getcwd() + os.sep + "plots/"

    if not os.path.exists(plots_dir):
        try:
            os.mkdir(plots_dir)
        except OSError as err:
            if err.errno == errno.EEXIST:
                print(plots_dir + " exists already, skipping creation")
            else:
                print(plots_dir + " could not be created.")

    data = np.loadtxt(file, dtype=float)
    t = arange(300000, 350000, 1, np.int32)


    # fig = figure(figsize=(32, 18), dpi=200)
    # for i in range(1):
    i = 2
    plot(t, data[300000:350000, 1], linewidth=1.0)
    xlabel('Time (unix epoch)')
    ylabel('gravity')
    title(plot_title)
    grid(True)
    savefig(plots_dir + os.path.basename(file) + "_" + str(i) + ".png")
    #clf()
    show()


if __name__ == '__main__':
    main(sys.argv)




