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

    file_name = argv[1]
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

    data = np.loadtxt(file_name, dtype=float)

#    X = arange(0, len(data[1, :]))
#    Y = arange(0, len(data[:, 1]))
    X = arange(0,8);
    Y = arange(0,12);
    X, Y = meshgrid(X, Y)

    fig = figure(figsize=(5, 5), dpi=200)
    for i in range(len(data[:,1])):
        Z = resize(data[i, :], (len(X), len(Y)))
        image_file_name = plots_dir + out_name + str(i)
    #    ylabel("Time (1/2 seconds)")
    #    xlabel("405nm Well Index (Matched to Excel Columns A-CS)")
    #    title("Well Intensities (405nm Camera) for Duration of May 5th Test")
        pcolormesh(X, Y, Z)
        cbar = colorbar();
        cbar.set_label("Well Intensities")
        savefig(image_file_name)
        clf()


if __name__ == '__main__':
    main(sys.argv)

