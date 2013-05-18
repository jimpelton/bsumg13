
// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-05-05                                                                      
// ******************************************************************************
using System.Collections.Generic;
namespace uGCapture
{

    public enum StatusStr
    {
        STAT_ERR,
        STAT_FAIL,
        STAT_GOOD,

        STAT_ERR_405,
        STAT_ERR_485,

        STAT_FAIL_405,
        STAT_FAIL_485,

        STAT_GOOD_405,
        STAT_GOOD_485,

        STAT_FAIL_PHID_1018,
        STAT_DISC_PHID_1018,
        STAT_ATCH_PHID_1018,
        STAT_GOOD_PHID_1018,

        STAT_FAIL_PHID_TEMP,
        STAT_DISC_PHID_TEMP,
        STAT_ATCH_PHID_TEMP,
        STAT_GOOD_PHID_TEMP,

        STAT_FAIL_PHID_ACCL,
        STAT_DISC_PHID_ACCL,
        STAT_ATCH_PHID_ACCL,
        STAT_GOOD_PHID_ACCL,

        STAT_FAIL_PHID_SPTL,
        STAT_DISC_PHID_SPTL,
        STAT_ATCH_PHID_SPTL,
        STAT_GOOD_PHID_SPTL,

        STAT_FAIL_NI6008DAQ,
        STAT_DISC_NI6008DAQ,
        STAT_ATCH_NI6008DAQ,
        STAT_GOOD_NI6008DAQ
    }

    public enum CommandStr
    {
        CMD_NONE,
        CMD_NI_HEATER_ON,
        CMD_NI_HEATER_OFF,
        CMD_NI_LIGHT_1_1_ON,
        CMD_NI_LIGHT_1_1_OFF,
        CMD_NI_LIGHT_1_2_ON,
        CMD_NI_LIGHT_1_2_OFF,
        CMD_NI_LIGHT_2_1_ON,
        CMD_NI_LIGHT_2_1_OFF,
        CMD_NI_LIGHT_2_2_ON,
        CMD_NI_LIGHT_2_2_OFF
    }

    public enum ErrStr 
    {
        INIT_FAIL_PHID_1018,
        INIT_FAIL_PHID_SPTL,
        INIT_FAIL_PHID_ACCEL,
	    INIT_FAIL_PHID_TEMP,
        INIT_FAIL_NI_6008,
        INIT_FAIL_APTINA,
        INIT_FAIL_VCOMM,
        INIT_FAIL_WRITER,
        INIT_FAIL_UPS,
	    INIT_FAIL_LOGGER
    }

    public enum MsgStr
    {
        INIT_OK_PHID_1018,
        INIT_OK_PHID_SPTL,
        INIT_OK_PHID_ACCEL,
	    INIT_OK_PHID_TEMP,  //unused
        INIT_OK_NI_6008,
        INIT_OK_APTINA,
        INIT_OK_VCOMM,
        INIT_OK_UPS,
	    INIT_OK_LOGGER
    }

    /// <summary>
    /// Id's unique to each receiver.
    /// Use these in conjunction with Str.GetMsgStr()
    /// </summary>
    public enum IdStr
    {
        ID_WRITER=0,          
        ID_APTINA_ONE,      
        ID_APTINA_TWO,      
        ID_PHIDGETS_1018,    
        ID_PHIDGETS_ACCEL,  
        ID_PHIDGETS_SPATIAL,
        ID_VCOMM,    
        ID_NI_DAQ,
        ID_UPS,
	    ID_LOGGER
    }
    
    public enum DirStr
    {
        DIR_WRITER,
        DIR_CAMERA405,
        DIR_CAMERA485,
        DIR_PHIDGETS,
        DIR_SPATIAL,
        DIR_VCOMM,
        DIR_NI_DAQ,
        DIR_UPS,
        DIR_LOGGER,
        DIR_ACCEL
    }

    public class Str
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

        public static string GetIdStr(IdStr id)
        {
            switch (id)
            {
                case IdStr.ID_WRITER: return "Writer"; 
                case IdStr.ID_APTINA_ONE: return "Aptina_One"; 
                case IdStr.ID_APTINA_TWO: return "Aptina_Two"; 
                case IdStr.ID_PHIDGETS_1018: return "Phidgets"; 
                case IdStr.ID_PHIDGETS_ACCEL: return "Phidgets_Accel"; 
                case IdStr.ID_PHIDGETS_SPATIAL: return "Phidgets_Spatial"; 
                case IdStr.ID_VCOMM: return "Weatherboard"; 
                case IdStr.ID_NI_DAQ: return "NI6008";
                case IdStr.ID_UPS: return "Ups";
		        case IdStr.ID_LOGGER: return "Logger";
                default: return "UnknownId";
            }
        }

        /// <summary>
        /// Maps a controller
        /// </summary>
        public static readonly Dictionary<DirStr, string> Dirs = new Dictionary<DirStr, string>()
        {
            { DirStr.DIR_CAMERA405,   "Camera405\\" }, 
            { DirStr.DIR_CAMERA485,   "Camera485\\" }, 
            { DirStr.DIR_PHIDGETS,    "Phidgets\\"  }, 
            { DirStr.DIR_SPATIAL,     "Spatial\\"   },
            { DirStr.DIR_ACCEL,       "Accel\\"     },
            { DirStr.DIR_VCOMM,       "Barometer\\" },
            { DirStr.DIR_NI_DAQ,      "NI6008\\"    }, 
            { DirStr.DIR_UPS,         "Ups\\"       },
            { DirStr.DIR_LOGGER,      "Log\\"       }
        };

        /// <summary>
        /// Maps a controller's data directory to the filename prefix used by writer.
        /// </summary>
        public static readonly Dictionary<DirStr, string> Pfx = new Dictionary<DirStr, string>()
            {
              { DirStr.DIR_CAMERA405,  "Camera405"    },
              { DirStr.DIR_CAMERA485,  "Camera485"  },
              { DirStr.DIR_PHIDGETS,   "Phidgets"     },
              { DirStr.DIR_SPATIAL,    "Spatial"   },
              { DirStr.DIR_ACCEL,      "Accel" },
              { DirStr.DIR_VCOMM,      "Barometer" },
              { DirStr.DIR_NI_DAQ,     "NI6008" },
              { DirStr.DIR_UPS,        "UPS"       },
              { DirStr.DIR_LOGGER,     "Log"       }
        };



        //public static string GetDirStr(DirStr dir)
        //{
        //    return Dirs[dir];
        //}
    }
}




