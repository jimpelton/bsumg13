__author__ = 'jim'

import unittest

import ugDataReader
import ugDataFile

class MyTestCase(unittest.TestCase):

    def test_read(self):
        dir405="testdat/dir405"
        dir485="testdat/dir485/"
        dirgrav="testdat/dirgrav/"
        dirout="testdat/dirout/"

        df=ugDataFile.ugDataFile(dir405,dir485,dirgrav,dirout)
        df.update()

        dr=ugDataReader.ugDataReader(df)
        dr.update()

        self.assertEqual(len(dr.valuesList("405")), 100)
        self.assertEqual(len(dr.valuesList("485")), 100)
        self.assertEqual(len(dr.valuesList("grav")), 100)


if __name__ == '__main__':
    unittest.main()
