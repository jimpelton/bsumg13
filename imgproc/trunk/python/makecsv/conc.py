__author__ = 'jim'

from re import match
import numpy as np


def _concRmin(ratios):
    """
    Find the minimum ratio
    """

def calculateConcentrations(ratios, val405, val485):
    """
    Calculate Ca+2 concentrations from calibration equation:
                 (R-Rmin)
      (Kd * Q) * --------
                 (Rmax-R)

    :param ratios: the well ratios on t=[0..tmax]
    :param val405: 405 well values on t=[0..tmax]
    :param val485: 485 well values on t=[0..tmax]
    """
    print("Calculating Concentrations...")
    shortest = min(len(ratios), len(val405), len(val485))
    Kd = 0.23  # dissosiation constant for indo-1 dye.
    carrs = concArrays()
    for time in range(shortest):
        well = 0
        for r in ratios[time]:
            if well >= 60:
                break

            egtaSlice = getEgta(well)
            ionoSlice = getIono(well)

            F405min = min(val405[time][egtaSlice])
            F405max = max(val405[time][ionoSlice])  # should be Iono
            F485min = min(val485[time][ionoSlice])
            F485max = max(val485[time][egtaSlice])  # should be EGTA
            Q = F485min / F485max
            Rmin = F405min / F485min
            Rmax = F405max / F485max
            numval = r - Rmin
            denval = Rmax - r

            carrs.QVals[time][well] = Q
            carrs.RminVals[time][well] = Rmin
            carrs.RmaxVals[time][well] = Rmax
            carrs.F405MinVals[time][well] = F405min
            carrs.F485MinVals[time][well] = F485min
            carrs.F405MaxVals[time][well] = F405max
            carrs.F485MaxVals[time][well] = F485max
            carrs.NumVals[time][well] = numval
            carrs.DenVals[time][well] = denval
            carrs.Concs[time][well] = Kd * Q * ((r - Rmin) / (Rmax - r))

            well += 1

    return carrs


def getEgta(wellIdx):
    cc = '0[0-3]|1[2-5]|2[4-7]|3[6-9]|4[8-9]|5[0-1]'  #co cul
    mc = '0[4-7]|1[6-9]|2[8-9]|3[0-1]|4[0-3]|5[2-5]'  #mc-3t3
    ml = '0[8-9]|1[0-1]|2[0-3]|3[2-5]|4[4-7]|5[6-9]'  #mlo-y4

    wellIdx = str(wellIdx).zfill(2)
    if match(cc, wellIdx):
        return slice(72, 76, 1)
    elif match(mc, wellIdx):
        return slice(76, 80, 1)
    elif match(ml, wellIdx):
        return slice(80, 84, 1)
    else:
        return None


def getIono(wellIdx):
    cc = '^0[0-3]|1[2-5]|2[4-7]|3[6-9]|4[8-9]|5[0-1]$'  #co cul
    mc = '^0[4-7]|1[6-9]|2[8-9]|3[0-1]|4[0-3]|5[2-5]$'  #mc-3t3
    ml = '^0[8-9]|1[0-1]|2[0-3]|3[2-5]|4[4-7]|5[6-9]$'  #mlo-y4

    wellIdx = str(wellIdx).zfill(2)
    if match(cc, wellIdx):
        return slice(60, 64, 1)
    elif match(mc, wellIdx):
        return slice(64, 68, 1)
    elif match(ml, wellIdx):
        return slice(68, 72, 1)
    else:
        return None


class concArrays():
    def __init__(self):
        # concentration stats
        self.QVals = np.zeros((7526, 60), dtype=np.float64)
        self.RminVals = np.zeros((7526, 60), dtype=np.float64)
        self.RmaxVals = np.zeros((7526, 60), dtype=np.float64)
        self.F405MinVals = np.zeros((7526, 60), dtype=np.float64)
        self.F485MinVals = np.zeros((7526, 60), dtype=np.float64)
        self.F405MaxVals = np.zeros((7526, 60), dtype=np.float64)
        self.F485MaxVals = np.zeros((7526, 60), dtype=np.float64)
        self.NumVals = np.zeros((7526, 60), dtype=np.float64)
        self.DenVals = np.zeros((7526, 60), dtype=np.float64)
        self.Concs = np.zeros((7526, 60), dtype=np.float64)