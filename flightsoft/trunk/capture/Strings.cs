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
        INIT_FAIL_VCOMM,
        INIT_FAIL_WRITER,
        INIT_FAIL_UPS
    }

    enum MsgStr
    {
        INIT_OK_PHID_1018,
        INIT_OK_PHID_SPTL,
        INIT_OK_PHID_ACCEL,
        INIT_OK_NI_6008,
        INIT_OK_APTINA,
        INIT_OK_VCOMM,
        INIT_OK_UPS
    }

    enum IdStr
    {
        ID_WRITER,          
        ID_APTINA_ONE,      
        ID_APTINA_TWO,      
        ID_PHIDGETS_1018,    
        ID_PHIDGETS_ACCEL,  
        ID_PHIDGETS_SPATIAL,
        ID_VCOMM,    
        ID_NI_DAQ,
        ID_UPS   
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
                case ErrStr.INIT_FAIL_VCOMM:
                    return "Weatherboard failed to initialize.";
                case ErrStr.INIT_FAIL_WRITER:
                    return "Writer failed to initialize.";
                case ErrStr.INIT_FAIL_UPS:
                    return "UPSController failed to initialize.";
                default:
                    return "Unknown Error.";
            }
        }

        public static string GetMsgStr(MsgStr msg)
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
                case MsgStr.INIT_OK_VCOMM:
                    return "Weatherboard initialized.";
                case MsgStr.INIT_OK_UPS:
                    return "UPSController initialized.";
                default:
                    return "Yar! There be pirates in this message! (Unknown Message).";
            }
        }

        public static string GetIdStr(IdStr msg)
        {
            switch (msg)
            {
                case IdStr.ID_WRITER: return "Writer"; 
                case IdStr.ID_APTINA_ONE: return "Aptina_One"; 
                case IdStr.ID_APTINA_TWO: return "Aptina_Two"; 
                case IdStr.ID_PHIDGETS_1018: return "Phidgets"; 
                case IdStr.ID_PHIDGETS_ACCEL: return "Phidgets_Accel"; 
                case IdStr.ID_PHIDGETS_SPATIAL: return "Phidgets_Spatial"; 
                case IdStr.ID_VCOMM: return "Weatherboard"; 
                case IdStr.ID_NI_DAQ: return "NI6008";
                case IdStr.ID_UPS: return "UPS"; 
                default: return "UnknownId";
            }
        }
    }
}




