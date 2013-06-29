import ugDataFile

__author__ = 'jim'

import unittest


class MyTestCase(unittest.TestCase):
    def setupDataFile(self):
        df = ugDataFile.ugDataFile(layout='testdata/28April2013_plate_layout_transposed_flipped.csv',
                                   outdir="testdata/dirout/",
                                   dir405="testdata/dir405/",
                                   dir485="testdata/dir485/",
                                   dirgrav="testdata/dirgrav/")
        return df

    def test_datafile(self):
        dir405 = "testdata/dir405"
        dir485 = "testdata/dir485"
        # dirgrav = "testdata/dirgrav"
        dirout = "testdata/dirout"

        df = self.setupDataFile()
        df.update()

        self.assertTrue(df.fileNames("dir405") is not None)
        self.assertTrue(df.fileNames("dir485") is not None)
        self.assertTrue(df.fileNames("dirgrav") is not None)
        self.assertTrue(df.fileNames("") is not None)

        self.assertEqual(len(df.fileNames("dir405")), 46)
        self.assertEqual(len(df.fileNames("dir485")), 46)
        self.assertEqual(len(df.fileNames("dirgrav")), 0)
        # self.assertEqual(df.sIdx(), 0)
        # self.assertEqual(df.eIdx(), 45)
        # self.assertEqual(df.length(), 46)

    def test_datafileFromTo(self):
        df = self.setupDataFile()

        df.fromTo(0, 10)
        df.update()

        self.assertEqual(df.sIdx(), 0)
        self.assertEqual(df.eIdx(), 10)
        self.assertEqual(len(df.fileNames("dir405")), 11)
        self.assertEqual(len(df.fileNames("dir485")), 11)
        self.assertEqual(len(df.fileNames("dirgrav")), 0)

    def test_accessors(self):
        df = self.setupDataFile()
        df.update()

        self.assertEqual("testdata/dirout/", df.dirout())
        self.assertEqual("testdata/dir405/", df.dir405())
        self.assertEqual("testdata/dir485/", df.dir485())
        self.assertEqual("testdata/dirgrav/", df.dirgrav())


        # def test_sanitize(self):
        #     dir405 = "testdata/dir405"
        #     dir485 = "testdata/dir485/"
        #     dirgrav = "testdata/dirgrav/"
        #     dirout = "testdata/dir"
        #
        #     with self.assertRaises(Exception) as eek:
        #         print(eek)
        #         df = ugDataFile.ugDataFile(dir405=dir405, dir485=dir485, dirgrav=dirgrav, outdir=dirout)
        #         df.update()


if __name__ == '__main__':
    unittest.main()
