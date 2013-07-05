__author__ = 'jim'

import unittest
import ugDataReader
import ugDataFile

import numpy as np
import pandas as pd


class DataReader2013Test(unittest.TestCase):

    def _isnumber(self, nstr):
        try:
            int(nstr)
            return True
        except:
            return False

    def setupDataFile_2013(self):
        return ugDataFile.ugDataFile(format_year=2013,
                                     dir405="testdata_2013/dir405",
                                     dir485="testdata_2013/dir485",
                                     dirgrav="testdata_2013/dirgrav")

    def test_read_405_2013(self):
        df = ugDataFile.ugDataFile(dir405="testdata_2013/dir405/")
        df.fromTo(0, 45)
        df.update()
        dr = ugDataReader.ugDataReader(format_year=2013,
                                       datafile=df,
                                       num_wells=192)
        dr.update()

        #the df loaded 46 files, each with 192 well values.
        #(see df.fromTo(0, 45) line above)
        dataFrame = dr.valuesList("405")
        self.assertIsInstance(dataFrame, pd.DataFrame)

        index = dataFrame.index
        self.assertEqual(len(index), 46)

        self.assertEqual(np.prod(dataFrame.shape), 46*192)
        with open("testdata_2013/dir405/time_values.txt", 'r') as tf:
            lines = tf.readlines()
            intLines = [int(x) for x in lines if self._isnumber(x)]
            self.assertEquals([x for x in index],
                              [x for x in intLines])


    def test_read_485_2013(self):
        df = ugDataFile.ugDataFile(dir485="testdata_2013/dir485/",
                                   format_year=2013)
        df.fromTo(0, 45)
        df.update()
        dr = ugDataReader.ugDataReader(format_year=2013,
                                       datafile=df,
                                       num_wells=192)
        dr.update()

        #the df loaded 46 files, each with 192 well values.
        #(see df.fromTo(0, 45) line above)
        dataFrame = dr.valuesList("485")
        self.assertIsInstance(dataFrame, pd.DataFrame)

        index = dataFrame.index
        self.assertEqual(len(index), 46)

        self.assertEqual(np.prod(dataFrame.shape), 46*192)
        with open("testdata_2013/dir485/time_values.txt", 'r') as tf:
            lines = tf.readlines()
            intLines = [int(x) for x in lines if self._isnumber(x)]
            self.assertEquals([x for x in index],
                              [x for x in intLines])

    def test_read_2013(self):
        df = self.setupDataFile_2013()
        df.fromTo(0, 45)
        df.update()
        dr = ugDataReader.ugDataReader(format_year=2013, datafile=df, num_wells=192)
        dr.update()

        # the df loaded 46 files, each with 192 well values.
        # (see df.fromTo(0, 45) line above)
        self.assertEqual(np.prod(dr.valuesList("405").shape), 46 * 192)
        self.assertEqual(np.prod(dr.valuesList("485").shape), 46 * 192)
        self.assertEqual(np.prod(dr.valuesList("grav").shape), 2154)

    def test_read_grav_2013(self):
        df = ugDataFile.ugDataFile(dirgrav="testdata_2013/dirgrav")
        df.fromTo(0,45)
        df.update()
        dr = ugDataReader.ugDataReader(format_year=2013, datafile=df, num_wells=192)
        dr.update()
        
        self.assertEqual(np.prod(dr.valuesList("grav").shape), 2154)

    def test_read_spat_2013(self):
        df = ugDataFile.ugDataFile(dirspat="testdata_2013/dirspat")
        df.fromTo(0,45)
        df.update()
        dr = ugDataReader.ugDataReader(format_year=2013, datafile=df, num_wells=192)
        dr.update()
        
        self.assertEqual(len(dr.valuesList("spat").index), 2177)

    def test_read_phid_2013(self):
        df = ugDataFile.ugDataFile(dirphid="testdata_2013/dirphid")
        df.fromTo(0,45)
        df.update()
        dr = ugDataReader.ugDataReader(format_year=2013, datafile=df, num_wells=192)
        dr.update()

        self.assertEquals(len(dr.valuesList("phid").index), 578)


    def test_read_ni_2013(self):
        pass

    def test_read_ups_2013(self):
        pass

if __name__ == '__main__':
    unittest.main()
