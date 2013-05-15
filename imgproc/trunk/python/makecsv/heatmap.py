from IPython.core.pylabtools import figsize
from matplotlib import colorbar

__author__ = 'jim'

import os, errno
from pylab import *
import numpy as np


def main(argv):
    """

    :param argv:
    :return:
    """

    file = argv[1]
    out_name = argv[2]
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

    X = arange(0, len(data[1, :]))
    Y = arange(0, len(data[:, 1]))

    X, Y = meshgrid(X, Y)
    # Z = resize(data[1, :], (len(X), len(Y)))

    fig = figure(figsize=(10,10), dpi=200)
    ylabel("Time (1/2 seconds)")
    xlabel("405nm Well Index (Matched to Excel Columns A-CS)")
    title("Well Intensities (405nm Camera) for Duration of May 5th Test")

    pcolormesh(X, Y, data)

    cbar = colorbar()
    cbar.set_label("405nm Well Intensities")


    savefig(out_name)


if __name__ == '__main__':
    main(sys.argv)

