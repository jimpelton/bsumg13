__author__ = 'jim'

import os, errno
from pylab import *
import numpy as np



def human_sort(self, s):
    return [int(k) if k.isdigit() else k for k in re.split(self._NumReg, s)]


def main(argv):
    file = argv[0]
    plots_dir = os.getcwd() + os.sep + "plots/"
    if not os.path.exists(plots_dir):
        try:
            os.mkdir(plots_dir)
        except OSError as err:
            if err.errno == errno.EEXIST:
                print(plots_dir + " exists already, skipping creation")
            else:
                print(plots_dir + " could not be created.")

    data = np.file
    # files = os.listdir(files_dir)
    # files.sort(key=human_sort)

    # for f in files:
    # with open(file, 'r') as data:
    #     i = 0
    #     for row in data:
    #         values = row.split(' ')
    #         t = arange(0.0, float(len(values)), 1)
    #         plot(t, values, linewidth=1.0)
    #
    #         xlabel('time (1/2 seconds)')
    #         ylabel('Well Value')
    #         title('thangs')
    #         grid(True)
    #         savefig(plots_dir + os.path.basename(file) + "_" + str(i) + ".png")
    #         i += 1


if __name__ == '__main__':
    main(sys.argv[1:])




