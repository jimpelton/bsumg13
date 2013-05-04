__author__ = 'jim'

import unittest

import ugDataReader
import ugDataFile


class MyTestCase(unittest.TestCase):
    def setupDataFile(self):
        diroutput = "testdata/dirout/"
        csvfile = 'testdata/28April2013_plate_layout_transposed_flipped.csv'

        df = ugDataFile.ugDataFile(layout='testdata/28April2013_plate_layout_transposed_flipped.csv',
                                   outdir="testdata/dirout/",
                                   dir405="testdata/dir405",
                                   dir485="testdata/dir485/",
                                   dirgrav="testdata/dirgrav/")
        return df

    def test_read(self):
        df = self.setupDataFile()
        df.fromTo(0, 45)

        df.update()

        dr = ugDataReader.ugDataReader(df)

        dr.update()

        self.assertEqual(dr.valuesList("dir405").size, 46 * 96)
        self.assertEqual(dr.valuesList("dir485").size, 46 * 96)
        # self.assertEqual(dr.valuesList("dirgrav"))


    def test_csvread(self):
        df = self.setupDataFile()
        df.update()
        dr = ugDataReader.ugDataReader(df)
        dr.update()
        layout = dr.layout()
        i = 0


if __name__ == '__main__':
    unittest.main()
