using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uGCapture
{
    enum ErrStr 
    {
        INIT_FAIL_PHID_1018,
        INIT_FAIL_PHID_SPTL,
        INIT_FAIL_PHID_ACCEL,
        INIT_FAIL_NI_6008,
        INIT_FAIL_APTINA,
        INIT_FAIL_WEATHERBOARD
    }

    enum MsgStr
    {
        INIT_OK_PHID_1018,
        INIT_OK_PHID_SPTL,
        INIT_OK_PHID_ACCEL,
        INIT_OK_NI_6008,
        INIT_OK_APTINA,
        INIT_OK_WEATHERBOARD
    }

    enum IdStr
    {
        ID_WRITER,          
        ID_APTINA_ONE,      
        ID_APTINA_TWO,      
        ID_PHIDGETS_DAQ,    
        ID_PHIDGETS_ACCEL,  
        ID_PHIDGETS_SPATIAL,
        ID_WEATHERBOARD,    
        ID_NI_DAQ           
    }


    class Str
    {
        
        public static string GetErrStr(ErrStr msg)
        {
            switch (msg)
            {
                case ErrStr.INIT_FAIL_PHID_1018:
                    return "Phidgets 1018 DAQ failed to initialize."; 
                case ErrStr.INIT_FAIL_PHID_SPTL:
                    return "Phidgets Spatial failed to initialize."; 
                case ErrStr.INIT_FAIL_PHID_ACCEL:
                    return "Phidgets accelerometer failed to initialize."; 
                case ErrStr.INIT_FAIL_NI_6008:
                    return "NI-6008 DAQ failed to initialize."; 
                case ErrStr.INIT_FAIL_APTINA:
                    return "Aptina camera failed to initialize.";
                case ErrStr.INIT_FAIL_WEATHERBOARD:
                    return "Weatherboard failed to initialize.";
                default:
                    return "Unknown Error.";
            }
        }

        public static string GetMsgString(MsgStr msg)
        {
            switch (msg)
            {
                case MsgStr.INIT_OK_PHID_1018:
                    return "Phidgets 1018 initialized."; 
                case MsgStr.INIT_OK_PHID_SPTL:
                    return "Phidgets Spatial initialized."; 
                case MsgStr.INIT_OK_PHID_ACCEL:
                    return "Phidgets accelerometer initialized."; 
                case MsgStr.INIT_OK_NI_6008:
                    return "NI-6008 DAQ initialized."; 
                case MsgStr.INIT_OK_APTINA:
                    return "Aptina camera initialized.";
                case MsgStr.INIT_OK_WEATHERBOARD:
                    return "Weatherboard initialized.";
                default:
                    return "Yar! There be pirates in this message!.";
            }
        }

        public static string GetIdStr(IdStr msg)
        {
            switch (msg)
            {
                case IdStr.ID_WRITER: return "Writer"; 
                case IdStr.ID_APTINA_ONE: return "Aptina_One"; 
                case IdStr.ID_APTINA_TWO: return "Aptina_Two"; 
                case IdStr.ID_PHIDGETS_DAQ: return "Phidgets"; 
                case IdStr.ID_PHIDGETS_ACCEL: return "Phidgets_Accel"; 
                case IdStr.ID_PHIDGETS_SPATIAL: return "Phidgets_Spatial"; 
                case IdStr.ID_WEATHERBOARD: return "Weatherboard"; 
                case IdStr.ID_NI_DAQ: return "NI6008"; 
                default: return "UnknownId";
            }
        }
    }
}




