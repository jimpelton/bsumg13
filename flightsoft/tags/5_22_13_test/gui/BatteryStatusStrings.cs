
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
                case (2):
                    return "AC Power";
                case (3):
                    return "Fully Charged";
                case (4):
                    return "Low";
                case (5):
                    return "Critical";
                case (6): goto case (9);//all of these are different charging states                           
                case (7): goto case (9);
                case (8): goto case (9);
                case (9):
                    return "Charging";
                //skipped 10 ( undefined )
                case (11):
                    return "Part Charged";
                default:
                    return null;
            }
        }
    }
}
