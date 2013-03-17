__author__ = 'jim'

import numpy as np


def findRmin(ratios):
    for i in range(ratios.shape[0]):
        egtaRats = ratios[i][slice(60, 64, 1)]
        ionoRats = ratios[i][slice(72, 76, 1)]

    return