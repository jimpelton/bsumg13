__author__ = 'jim'

from re import match

class conc():

    QVals = []
    RminVals = []
    RmaxVals = []
    F405MinVals = []
    F485MinVals = []
    F405MaxVals = []
    F485MaxVals = []
    NumVals = []
    DenVals = []


    def calculateConcentrations(self, ratios, val405, val485):
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
        concs = []
        for time in range(shortest):
            well = 0
            Ca2_t = []
            qval = []
            rminval = []
            rmaxval = []
            f405minval = []
            f485minval = []
            f405maxval = []
            f485maxval = []
            numval = []
            denval = []

            for r in ratios[time]:
                if well >= 60:
                    break

                egtaSlice = self.getEgta(well)
                ionoSlice = self.getIono(well)

                F405min = min(val405[time][egtaSlice])
                F405max = max(val405[time][ionoSlice])  # should be Iono
                F485min = min(val485[time][ionoSlice])
                F485max = max(val485[time][egtaSlice])  # should be EGTA
                Q = F485min / F485max
                Rmin = F405min / F485min
                Rmax = F405max / F485max

                qval.append(Q)
                rminval.append(Rmin)
                rmaxval.append(Rmax)
                f405minval.append(F405min)
                f405maxval.append(F405max)
                f485minval.append(F485min)
                f485maxval.append(F485max)
                numval.append(r - Rmin)
                denval.append(Rmax - r)

                Ca2_t.append(Kd * Q * ((r - Rmin) / (Rmax - r)))
                well += 1

            self.QVals.append(qval)
            self.RminVals.append(rminval)
            self.RmaxVals.append(rmaxval)
            self.F405MinVals.append(f405minval)
            self.F485MinVals.append(f485minval)
            self.F405MaxVals.append(f405maxval)
            self.F485MaxVals.append(f485maxval)
            self.NumVals.append(numval)
            self.DenVals.append(denval)

            concs.append(Ca2_t)
        return concs


    def getEgta(self, wellIdx):
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


    def getIono(self, wellIdx):
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

