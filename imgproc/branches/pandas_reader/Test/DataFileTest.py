import ugDataFile

__author__ = 'jim'

import unittest


class MyTestCase(unittest.TestCase):
    def setupDataFile(self):
        df = ugDataFile.ugDataFile(layout='testdata/28April2013_plate_layout_transposed_flipped.csv',
                                   dirout="testdata/dirout/",
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
        df.fromTo(0, 45)
        df.update()

        self.assertTrue(df.filesList("405") is not None)
        self.assertTrue(df.filesList("485") is not None)
        self.assertTrue(df.filesList("grav") is not None)
        # self.assertTrue(df.filesList("") is not None)

        self.assertEqual(len(df.filesList("405")), 46)
        self.assertEqual(len(df.filesList("485")), 46)
        self.assertEqual(len(df.filesList("grav")), 0)
        # self.assertEqual(df.sIdx(), 0)
        # self.assertEqual(df.eIdx(), 45)
        # self.assertEqual(df.length(), 46)

    def test_datafileFromTo(self):
        df = self.setupDataFile()

        df.fromTo(0, 10)
        df.update()

        self.assertEqual(df.sIdx(), 0)
        self.assertEqual(df.eIdx(), 10)
        self.assertEqual(df.length(), 11)
        self.assertEqual(len(df.filesList("405")), 11)
        self.assertEqual(len(df.filesList("485")), 11)
        self.assertEqual(len(df.filesList("grav")), 0)

    def test_accessors(self):
        df = self.setupDataFile()
        df.update()

        self.assertEqual("testdata/dirout/", df.filesDir("out"))
        self.assertEqual("testdata/dir405/", df.filesDir("405"))
        self.assertEqual("testdata/dir485/", df.filesDir("485"))
        self.assertEqual("testdata/dirgrav/", df.filesDir("grav"))


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
