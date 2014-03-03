using System;
using System.Collections.Generic;
using System.IO;

namespace uGCapture 
{
    public struct Center 
    {
        int m_x,m_y;
        public Center(int x, int y)
        {
            m_x = x;
            m_y = y;
        }
        public int X
        {
            get{return  m_x; }
        }
        public int Y
        {
            get {return m_y;}
        }
    }


    public class CirclesFile
    {

        private Dictionary<int, Center> m_centers;
        private string m_filename;
        private int m_ximg;
        private int m_yimg;
        private int m_crad;

        /// <summary>
        /// Circle radius for the circles in this file.
        /// </summary>
        public int CRad 
        {
            get { return m_crad; }
        }

        /// <summary>
        /// Y-Dimension in pixels of the image that these circles
        /// are for.
        /// </summary>
        public int YImg
        {
            get { return m_yimg; }
        }

        /// <summary>
        /// X-Dimension in pixels of the image that these circles
        /// are for.
        /// </summary>
        public int XImg
        {
            get { return m_ximg; }
        }

        /// <summary>
        /// Filename that these circles are from.
        /// </summary>
        public string Filename
        {
            get { return m_filename; }
        }

        public CirclesFile()
        {
            m_centers = new Dictionary<int, Center>();
        }

        public Center this[int i]
        {
            get { return m_centers[i]; }
        }


        public int Open(string filename)
        {
            m_filename = filename;
            int nCirc, nLines; 
            nCirc = nLines = 0;
            StreamReader tr;

            try
            {
                tr = File.OpenText(filename);
                //string line;
                while (!tr.EndOfStream && nLines < 500)
                {
                    if (p_Line(tr.ReadLine(), nLines) == 1)
                        nLines += 1;
                    else break;
                }
                tr.Close();
            }
            catch (Exception eek)
            {
                Console.WriteLine(eek.StackTrace);
            }
            return m_centers.Count;
        }
        

        private int p_Line(string line, int lnum)
        {
            int rval = 1;
            int nKey;

            if (line == "") 
                return rval;

            line = line.Trim();
            
            string[] vals = line.Split(':');
            if (vals.Length < 2)
            {
                rval = 0;
                Console.WriteLine("Syntax error near line " + lnum);
            }
            else if (int.TryParse(vals[0].Trim(), out nKey))
                rval = p_WellLoc(vals[0], vals[1], lnum);
            else
                rval = p_ImgInfo(vals[0], vals[1], lnum);

            return rval;
        }

        // Parse a line containing a well location center.
        // returns 1 on success, 0 otherwise.
        private int p_WellLoc(string wellIdx, string wellCenter, int lnum)
        {
            int rval = 1;

            wellIdx = wellIdx.Trim();
            wellCenter = wellCenter.Trim();

            string[] vals = wellCenter.Split(',');

            if (vals.Length < 2)
            {
                rval = 0;
                Console.WriteLine("Syntax error near line " + lnum);
            }
            else
            {
                int nValue, n2Value, idx;
                if ( int.TryParse(vals[0], out nValue)   &&
                     int.TryParse(vals[1], out n2Value)  &&
                     int.TryParse(wellIdx, out idx) )
                {
                    m_centers[idx] = new Center(nValue, n2Value);
                }
                else
                {
                    rval = 0;
                    Console.WriteLine("Syntax error on line " + lnum + ". Expected 'int':'int','int'.");
                }
            }

            return rval;
        }

        // Parse a line containing ximg, yimg, or crad image information.
        // returns 1 on success, 0 otherwise.
        private int p_ImgInfo(string key, string value, int lnum)
        {
            int rval = 1;
            try
            {
                if (key == "imgx")
                {
                    m_ximg = int.Parse(value);
                }
                else if (key == "imgy")
                {
                    m_yimg = int.Parse(value);
                }
                else if (key == "crad")
                {
                    m_crad = int.Parse(value);
                }
                else
                {
                    Console.WriteLine("Syntax error in config file on line {2}: key: {0} value: {1}", key, value, lnum);
                    rval = 0;
                }
            }
            catch (FormatException eek)
            {
                rval = 0;
                String s = String.Format("Bad value for integer in config file on line {2}: key: {0} value: {1}", key, value, lnum);
                Console.WriteLine(s);
                Console.WriteLine(eek.StackTrace);
            }
            return rval;
        }

    }
}
