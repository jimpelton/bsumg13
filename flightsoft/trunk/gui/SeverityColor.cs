using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using uGCapture;
namespace gui
{
    class SeverityColor
    {
        private SeverityColor() { }

        public static int BLACK = 0;    /* no severity */
        public static int BLUE = 1;
        public static int VIOLET = 2;    /* somewhat severe*/
        public static int YELLOW = 3;
        public static int RED = 4;
        public static int ORANGE = 5;    /* hair on fire */

        private static Color[] severityColors = 
        {
            Color.Black,      
            Color.Blue,       
            Color.BlueViolet,  
            Color.Yellow, 
            Color.Red, 
            Color.Red   
        };


        /// <summary>
        /// Get the color which matches given severity index.
        /// If color > the highest severity level, return the color representing
        /// the max severity. Otherwise return the requested color. If color is
        /// less than 0 then returns the color representing the lowest severity level.
        /// </summary>
        /// <param name="color">The severity level (1-5)</param>
        /// <returns>A System.Drawing.Color for the given severity.</returns>
        public static Color GetColor(int color)
        {
            Color c = severityColors[0];

            if (color > 0 && color < severityColors.Length)
            {
                c = severityColors[color];
            }
            else if (color > severityColors.Length)
            {
                c = severityColors.Last();
            }

            return c;
        }
    }
}
