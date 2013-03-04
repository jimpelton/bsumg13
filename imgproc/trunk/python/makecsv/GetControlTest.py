from makecsv import getIono, getEgta

__author__ = 'jim'

import unittest



class MyTestCase(unittest.TestCase):
    def test_iono(self):
        slc = getIono(10)
        self.assertEqual(slc, slice(68,72,1))

    def test_egta(self):
        slc = getEgta(0)
        self.assertEqual(slc, slice(72,76,1))

    def test_egta_2(self):
        slc = getEgta(5)
        self.assertEqual(slc, slice(76,80,1))

    def test_egta_3(self):
        slc=getEgta(10)
        self.assertEqual(slc,slice(80,84,1))



if __name__ == '__main__':
    unittest.main()
