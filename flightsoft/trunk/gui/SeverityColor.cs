using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace gui
{
    class SeverityColor
    {
        private SeverityColor() { }

        static int BLACK    = 0;    /* no severity */      
        static int BLUE     = 1;     
        static int VIOLET   = 2;    /* somewhat severe*/
        static int YELLOW   = 3;
        static int RED      = 4;
        static int ORANGE   = 5;    /* hair on fire */

        private static Color[] severityColors = 
        {
            Color.Black,      
            Color.Blue,       
            Color.BlueViolet,  
            Color.Yellow, 
            Color.Red, 
            Color.OrangeRed   
        };

        /// <summary>
        /// Get the color which matches given severity index.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color GetColor(int color)
        {
            if (color > 0 && color < severityColors.Length)
            {
                return severityColors[color];
            }
            return severityColors[0];
        }
    }
}
