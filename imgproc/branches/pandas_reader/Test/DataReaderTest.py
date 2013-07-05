__author__ = 'jim'

import unittest
import pandas as pd
import numpy as np
import ugDataReader
import ugDataFile


class DataReaderTest(unittest.TestCase):
    def setupDataFile(self):
        return ugDataFile.ugDataFile(layout='testdata/28April2013_plate_layout_transposed_flipped.csv',
                                     dirout="testdata/dirout/",
                                     dir405="testdata/dir405",
                                     dir485="testdata/dir485/",
                                     dirgrav="testdata/dirgrav/")

    def test_read(self):
        df = self.setupDataFile()
        df.fromTo(0, 45)
        df.update()
        dr = ugDataReader.ugDataReader(format_year=2012,
                                       datafile=df,
                                       num_wells=96)
        dr.update()

        #the df loaded 46 files, each with 96 well values.
        #(see df.fromTo(0, 45) line above)

        self.assertIsInstance(dr.valuesList("405"), pd.DataFrame)
        self.assertIsInstance(dr.valuesList("485"), pd.DataFrame)

        self.assertEqual(np.prod(dr.valuesList("405").shape), 46 * 96)
        self.assertEqual(np.prod(dr.valuesList("485").shape), 46 * 96)
        # self.assertEqual(dr.valuesList("dirgrav"))

        # self.assertEqual(, 46 * 192)
        # self.assertEqual(dr.valuesList("485").size, 46 * 96)
        # self.assertEqual(dr.valuesList("dirgrav"))

    def test_timeStamp(self):
        df = ugDataFile.ugDataFile(dir405="testdata_2013/dir405",
                                   dir485="testdata_2013/dir485")
        df.fromTo(0, 45)
        df.update()
        dr = ugDataReader.ugDataReader(format_year=2013,
                                       datafile=df)
        dr.update()

        end = dr.valueTimes("405").index
        ts = dr.timeStringDeltaFromStart(end)
        self.assertEqual("29.354", ts)

    def test_csvread(self):
        df = self.setupDataFile()
        df.update()
        dr = ugDataReader.ugDataReader(format_year=2012, datafile=df, num_wells=96)
        dr.update()
        expected = {'200 uL co culture': [44, 45], 'MLO-Y4 on cytopore no dye': [9], '25 uL MLOY4': [19, 20],
                       '25 uL co culture': [23, 24], '50 uL MC3T3': [28, 29], 'MC3T3 on cytopore no dye': [10],
                       '100 uL MLOY4': [33, 34], '100 uL co culture': [37, 38], '400 uL co culture': [51, 52],
                       '50uL co cilture': [30, 31], 'Co-culture on cytopore no dye': [11], '200 uL MLOY4': [40, 41],
                       '400 uL MC3T3': [49, 50], '50 uL MLOY4': [26, 27], '100 uL MC3T3': [35, 36],
                       '25 uL MC3T3': [21, 22], '400 uL MLOY4': [47, 48], '200 uL MC3T3': [42, 43]}

        layout = dr.layout()
        self.assertDictEqual(layout, expected)

        print(layout)


        # def test_timestamp(self):
        # df = self.setupDateFile()
        # df.update()
        # dr = ugDataReader.ugDataReader(df)
        # dr.update()

# plate layout from April 28, 2013:
# '5': [18]
#  '4': [25]
#  '7': [16]
#  '6': [17]
#  '1': [46]
#  '25 uL MC3T3': [21 22]
#  '3': [32]
#  '2': [39]
#  'Co-culture on cytopore no dye': [11]
#  '25 uL MLOY4': [19 20]
#  '8': [15]
#  '100 uL MLOY4': [33 34]
#  '200 uL co culture': [44 45]
#  '50 uL MLOY4': [26 27]
#  '200 uL MLOY4': [40 41]
#  'MC3T3 on cytopore no dye': [10]
#  '400 uL MLOY4': [47 48]
#  '200 uL MC3T3': [42 43]
#  '400 uL MC3T3': [49 50]
#  '100 uL MC3T3': [35 36]
#  '100 uL co culture': [37 38]
#  'MLO-Y4 on cytopore no dye': [9]
#  '50 uL MC3T3': [28 29]
#  'H': [0]
#  'E': [3]
#  'D': [4]
#  'G': [1]
#  'F': [2]
#  'A': [7]
#  '11': [12]
#  'C': [5]
#  'B': [6]
#  '400 uL co culture': [51 52]
#  '50uL co cilture': [30 31]
#  '25 uL co culture': [23 24]
#  '12': [8]
#  '9': [14]
#  '10': [13]



if __name__ == '__main__':
    unittest.main()
