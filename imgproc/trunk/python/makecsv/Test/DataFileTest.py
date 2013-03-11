import ugDataFile

__author__ = 'jim'

import unittest


class MyTestCase(unittest.TestCase):

    def test_datafile(self):
        dir405="testdat/dir405"
        dir485="testdat/dir485"
        dirgrav="testdat/dirgrav"
        dirout="testdat/dirout"

        df=ugDataFile.ugDataFile(dir405,dir485,dirgrav,dirout)
        df.update()

        self.assertEqual(len(df.fileNames("405")),  100)
        self.assertEqual(len(df.fileNames("485")),  100)
        self.assertEqual(len(df.fileNames("grav")), 100)
        self.assertEqual(df.sIdx(), 0)
        self.assertEqual(df.eIdx(), 99)
        self.assertEqual(df.length(), 100)

    def test_datafileFromTo(self):
        dir405="testdat/dir405"
        dir485="testdat/dir485"
        dirgrav="testdat/dirgrav"
        dirout="testdat/dirout"

        df=ugDataFile.ugDataFile(dir405,dir485,dirgrav,dirout)
        df.fromTo(0,10)
        df.update()

        self.assertEqual(df.sIdx(),0)
        self.assertEqual(df.eIdx(),10)
        self.assertTrue(len(df.fileNames("405")), 11)
        self.assertTrue(len(df.fileNames("485")), 11)
        self.assertTrue(len(df.fileNames("grav")), 11)


    def test_sanitize(self):
        dir405="testdat/dir405"
        dir485="testdat/dir485/"
        dirgrav="testdat/dirgrav/"
        dirout="testdat/dir"

        with self.assertRaises(Exception) as eek:
            print(eek)
            df=ugDataFile.ugDataFile(dir405,dir485,dirgrav,dirout)
            df.update()



if __name__ == '__main__':
    unittest.main()
