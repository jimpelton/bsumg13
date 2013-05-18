using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gui
{
    class BatteryStatusStrings
    {
        public static string GetBatteryStatusStr(int m)
        {
            switch (m)
            {
                case (1):
                    return "Discharging";
                    break;
                case (2):
                    return "AC Power";
                    break;
                case (3):
                    return "Fully Charged";
                    break;
                case (4):
                    return "Low";
                    break;
                case (5):
                    return "Critical";
                    break;
                case (6): goto case (9);//all of these are different charging states                           
                case (7): goto case (9);
                case (8): goto case (9);
                case (9):
                    return "Charging";
                    break;
                //skipped 10 ( undefined )
                case (11):
                    return "Part Charged";
                    break;
                default:
                    return null;
            }
        }
    }
}
