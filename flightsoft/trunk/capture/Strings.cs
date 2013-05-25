
// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-05-05                                                                      
// ******************************************************************************
using System.Collections.Generic;
namespace uGCapture
{
    public enum Status
    {
        STAT_NONE=0,       // status default, or N/A, or never given.
        STAT_ERR,        // device error
        STAT_FAIL,       // device failed
        STAT_GOOD,       // device status ok
        STAT_DISC,       // device disconnected
        STAT_ATCH,       // device was attached

        STAT_ERR_405,
        STAT_ERR_485,

        STAT_FAIL_405,
        STAT_FAIL_485,

        STAT_GOOD_405,
        STAT_GOOD_485
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
        ERR_NONE = 0,
        // INIT FAIL 
        INIT_FAIL_PHID_1018,
        INIT_FAIL_PHID_SPTL,
        INIT_FAIL_PHID_ACCEL,
	    INIT_FAIL_PHID_TEMP,
        INIT_FAIL_NI_6008,
        
        INIT_FAIL_APTINA,
        INIT_FAIL_APTINA_INITMIDLIB,    
        INIT_FAIL_APTINA_OPENTRANSPORT,  
        INIT_FAIL_APTINA_ZERO_SIZE,
        INIT_FAIL_APTINA_FUSE_ERROR,
        INIT_FAIL_APTINA_KEY_NOT_SUPPORTED,
        INIT_FAIL_APTINA_INI_LOAD_ERROR,
        INIT_FAIL_APTINA_INI_POLLREG_TIMEOUT,
         

        INIT_FAIL_VCOMM,
        INIT_FAIL_WRITER,
        INIT_FAIL_UPS,
	    INIT_FAIL_LOGGER,
        
        // INIT SUCCESS 
        INIT_OK_PHID_1018,
        INIT_OK_PHID_SPTL,
        INIT_OK_PHID_ACCEL,
	    INIT_OK_PHID_TEMP,  //unused
        INIT_OK_NI_6008,
        INIT_OK_APTINA,
        INIT_OK_VCOMM,
        INIT_OK_UPS,
	    INIT_OK_LOGGER,

        // APTINA
        APTINA_DISCONNECT,
        APTINA_RECONNECT,
        APTINA_FAIL_CAPTURE_NULLBUFFER,
        APTINA_FAIL_CAPTURE,

        //PHIDGITS
        PHID_1018_STAT_OK,
        PHID_1018_STAT_DISC,
        PHID_1018_STAT_ATCH,
        PHID_1018_STAT_FAIL,
        
        //PHID TEMP
        PHID_TEMP_STAT_OK,
        PHID_TEMP_STAT_DISC,
        PHID_TEMP_STAT_ATCH,
        PHID_TEMP_STAT_FAIL,

        //PHID SPTL
        PHID_SPTL_STAT_DISC,
        PHID_SPTL_STAT_ATCH,
        PHID_SPTL_STAT_FAIL,

        NI6008_STAT_FAIL,
        
        UPS_ERR_NOT_FOUND,
        UPS_ERR_STATUS_PROPERTY_NOT_FOUND,

        // WRITER
        WRITER_FAIL_CREATE_DIRS,
        WRITER_FAIL_WRITE_BUFFER,
        WRITER_OK_CREATE_DIRS,
        WRITER_OK_WRITE_BUFFER,
        WRITER_OK_EXIT_LOOP
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
                case ErrStr.ERR_NONE:                        return "";
                case ErrStr.INIT_FAIL_PHID_1018:              return "Phidgets 1018 DAQ failed to initialize."; 
                case ErrStr.INIT_FAIL_PHID_SPTL:              return "Phidgets Spatial failed to initialize."; 
                case ErrStr.INIT_FAIL_PHID_ACCEL:             return "Phidgets accelerometer failed to initialize."; 
                case ErrStr.INIT_FAIL_PHID_TEMP:              return "Phidgets temp sensor failed to initialize.";

                case ErrStr.INIT_FAIL_NI_6008:                return "NI-6008 DAQ failed to initialize."; 

                case ErrStr.INIT_FAIL_APTINA:                 return "Aptina camera failed to initialize.";
                case ErrStr.INIT_FAIL_APTINA_INITMIDLIB:      return "Midlib2 failed to initialize.";
                case ErrStr.INIT_FAIL_APTINA_OPENTRANSPORT:   return "Midlib2 opentransport failed.";
                case ErrStr.INIT_FAIL_APTINA_ZERO_SIZE:       return "Midlib2 returned that frames are 0 sized.";
                case ErrStr.INIT_FAIL_APTINA_FUSE_ERROR:      return "Returned wavelength was 0 during aptinaController.initMidlib(). Fuse register not read correctly?";
               
                case ErrStr.INIT_FAIL_VCOMM:                  return "Weatherboard failed to initialize.";
                
                case ErrStr.INIT_FAIL_WRITER:                 return "Writer failed to initialize.";

                case ErrStr.INIT_FAIL_UPS:                    return "UPSController failed to initialize.";

                case ErrStr.INIT_OK_PHID_1018:                return "Phidgets 1018 initialized.";
                case ErrStr.INIT_OK_PHID_SPTL:                return "Phidgets Spatial initialized.";
                case ErrStr.INIT_OK_PHID_ACCEL:               return "Phidgets accelerometer initialized.";
                case ErrStr.INIT_OK_NI_6008:                  return "NI-6008 DAQ initialized.";
                case ErrStr.INIT_OK_APTINA:                   return "Aptina camera initialized.";
                case ErrStr.INIT_OK_VCOMM:                    return "Weatherboard initialized.";
                case ErrStr.INIT_OK_UPS:                      return "UPSController initialized.";

                case ErrStr.APTINA_FAIL_CAPTURE_NULLBUFFER:   return "Midlib2 getFrame() returned null pointer.";
                case ErrStr.APTINA_FAIL_CAPTURE:              return "Aptina capture failed.";
                case ErrStr.APTINA_DISCONNECT:                return "Midlib detected camera removal.";
                case ErrStr.APTINA_RECONNECT:                 return "Aptina camera reconnected after being disconnected.";
                
                case ErrStr.PHID_1018_STAT_OK:                return "Phidgits 1018 reports good status.";
                case ErrStr.PHID_1018_STAT_DISC:              return "Phidgits 1080 disconnected.";
                case ErrStr.PHID_1018_STAT_ATCH:              return "Phidgits 1018 attached.";
                case ErrStr.PHID_1018_STAT_FAIL:              return "Phidgits 1018 failure!.";

                case ErrStr.PHID_TEMP_STAT_OK:                return "Phidgits temperature probe reports good status.";
                case ErrStr.PHID_TEMP_STAT_DISC:              return "Phidgits Temperature probe diconnected.";
                case ErrStr.PHID_TEMP_STAT_ATCH:              return "Phidgits temperature probe attached.";
                case ErrStr.PHID_TEMP_STAT_FAIL:              return "Phidgits temperature probe failed!.";


                case ErrStr.NI6008_STAT_FAIL:                 return "NI-6008 failure!.";

                case ErrStr.UPS_ERR_NOT_FOUND:                return "A ups was not found!";
                case ErrStr.UPS_ERR_STATUS_PROPERTY_NOT_FOUND:return "UPS status property is unavailable.";
                
                case ErrStr.WRITER_OK_CREATE_DIRS:            return "Writer successfully created storage directory.";
                case ErrStr.WRITER_FAIL_WRITE_BUFFER:         return "Writer failed to write a buffer: ";
                case ErrStr.WRITER_FAIL_CREATE_DIRS:          return "Writer failed to create storage directory.";
                case ErrStr.WRITER_OK_WRITE_BUFFER:           return "Writer wrote buffer: ";
                case ErrStr.WRITER_OK_EXIT_LOOP:              return "Writer exited write loop.";

                default:                                      return "Unknown Error.";
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
              { DirStr.DIR_CAMERA485,  "Camera485"    },
              { DirStr.DIR_PHIDGETS,   "Phidgets"     },
              { DirStr.DIR_SPATIAL,    "Spatial"      },
              { DirStr.DIR_ACCEL,      "Accel"        },
              { DirStr.DIR_VCOMM,      "Barometer"    },
              { DirStr.DIR_NI_DAQ,     "NI6008"       },
              { DirStr.DIR_UPS,        "UPS"          },
              { DirStr.DIR_LOGGER,     "Log"          }
        };

        public static readonly Dictionary<MiErrorCode, string> MiErrStr = new Dictionary<MiErrorCode, string>()
            {
               { MiErrorCode.MI_CAMERA_SUCCESS,        "MI Succes"                                                                   },
               { MiErrorCode.MI_CAMERA_ERROR,         "MI General failure for midlib routines"                                       },
               { MiErrorCode.MI_GRAB_FRAME_ERROR ,         "General failure for grab frame routine"                                  },
               { MiErrorCode.MI_NOT_ENOUGH_DATA_ERROR ,         "Grab frame failed to return enough data "                           },
               { MiErrorCode.MI_EOF_MARKER_ERROR ,         "EOF packet not found in grab frame data"                                 },
               { MiErrorCode.MI_BUFFER_SIZE_ERROR,         "GrabFrame buffer is too small "                                          },
               { MiErrorCode.MI_SENSOR_FILE_PARSE_ERROR,         "There was an error parsing the sdat file"                          },
               { MiErrorCode.MI_SENSOR_DOES_NOT_MATCH,         "Cannot find sdat file which matches sensor "                         },
               { MiErrorCode.MI_SENSOR_NOT_INITIALIZED,         "The sensor structure has not been initialized (call updateFrame)"   },
               { MiErrorCode.MI_SENSOR_NOT_SUPPORTED,         "The sensor is no longer supported"                                    },
               { MiErrorCode.MI_I2C_BIT_ERROR ,         "I2C bit error "                                                             },
               { MiErrorCode.MI_I2C_NACK_ERROR ,         "I2C NAC error"                                                             },
               { MiErrorCode.MI_I2C_TIMEOUT ,         "I2C time out error"                                                           },
               { MiErrorCode.MI_CAMERA_TIMEOUT ,       "Timeout waiting for camera"                                                  },
               { MiErrorCode.MI_TOO_MUCH_DATA_ERROR ,         "Grab frame returned more data than expected"                          },
               { MiErrorCode.MI_CAMERA_NOT_SUPPORTED ,         "The function call is not supported"                                  },
               { MiErrorCode.MI_PARSE_SUCCESS ,         "Parsing was successful"                                                     },
               { MiErrorCode.MI_DUPLICATE_DESC_ERROR,         "Duplicate unique descriptor was found"                                },
               { MiErrorCode.MI_PARSE_FILE_ERROR,         "Unable to open sensor data file"                                          },
               { MiErrorCode.MI_PARSE_REG_ERROR,         "Error parsing the register descriptors"                                    },
               { MiErrorCode.MI_UKNOWN_SECTION_ERROR,         "Unknown Section found in sensor data file"                            },
               { MiErrorCode.MI_CHIP_DESC_ERROR ,         "Error parsing the chip descriptor section"                                },
               { MiErrorCode.MI_PARSE_ADDR_SPACE_ERROR,         "Error parsing the address space section"                            },
               { MiErrorCode.MI_INI_SUCCESS,        "INI Preset is loaded successfully"                                              },
               { MiErrorCode.MI_INI_KEY_NOT_SUPPORTED,        "Key is not supported - will be ignored "                              },
               { MiErrorCode.MI_INI_LOAD_ERROR ,        "Error loading INI preset     "                                              },
               { MiErrorCode.MI_INI_POLLREG_TIMEOUT ,        "time out in POLLREG/POLL_VAR/POLL_FIELD command"                       },
               { MiErrorCode.MI_INI_HANDLED_SUCCESS,        "transport handled the command, success"                                 },
               { MiErrorCode.MI_INI_HANDLED_ERROR ,        "transport handled the command, with error"                               },
               { MiErrorCode.MI_INI_NOT_HANDLED ,        "transport did not handle the command"                                      }
            };  

        public enum MiErrorCode
        {
            //Generic return codes
            MI_CAMERA_SUCCESS = 0x00,         //Success value for midlib routines
            MI_CAMERA_ERROR = 0x01,         //General failure for midlib routines
            //Grabframe return codes
            MI_GRAB_FRAME_ERROR = 0x03,         //General failure for grab frame routine
            MI_NOT_ENOUGH_DATA_ERROR = 0x04,         //Grab frame failed to return enough data
            MI_EOF_MARKER_ERROR = 0x05,         //EOF packet not found in grab frame data
            MI_BUFFER_SIZE_ERROR = 0x06,         //GrabFrame buffer is too small
            //mi_OpenCameras return codes
            MI_SENSOR_FILE_PARSE_ERROR = 0x07,         //There was an error parsing the sdat file
            MI_SENSOR_DOES_NOT_MATCH = 0x08,         //Cannot find sdat file which matches sensor
            MI_SENSOR_NOT_INITIALIZED = 0x09,         //The sensor structure has not been initialized (call updateFrame)
            MI_SENSOR_NOT_SUPPORTED = 0x0A,         //The sensor is no longer supported
            //I2C return codes
            MI_I2C_BIT_ERROR = 0x0B,         //I2C bit error  
            MI_I2C_NACK_ERROR = 0x0C,         //I2C NAC error
            MI_I2C_TIMEOUT = 0x0D,         //I2C time out error
            MI_CAMERA_TIMEOUT = 0x0E,
            MI_TOO_MUCH_DATA_ERROR = 0x0F,         //Grab frame returned more data than expected

            MI_CAMERA_NOT_SUPPORTED = 0x10,         //The function call is not supported

            //return codes for parsing sdat file
            MI_PARSE_SUCCESS = 0x20,         //Parsing was successful
            MI_DUPLICATE_DESC_ERROR = 0x21,         //Duplicate unique descriptor was found
            MI_PARSE_FILE_ERROR = 0x22,         //Unable to open sensor data file
            MI_PARSE_REG_ERROR = 0x23,         //Error parsing the register descriptors
            MI_UKNOWN_SECTION_ERROR = 0x24,         //Unknown Section found in sensor data file
            MI_CHIP_DESC_ERROR = 0x25,         //Error parsing the chip descriptor section
            MI_PARSE_ADDR_SPACE_ERROR = 0x26,         //Error parsing the address space section
            //Error codes for loading INI presets 
            MI_INI_SUCCESS = 0x100,        //INI Preset is loaded successfully
            MI_INI_KEY_NOT_SUPPORTED = 0x101,        //Key is not supported - will be ignored
            MI_INI_LOAD_ERROR = 0x102,        //Error loading INI preset
            MI_INI_POLLREG_TIMEOUT = 0x103,        //time out in POLLREG/POLL_VAR/POLL_FIELD command
            MI_INI_HANDLED_SUCCESS = 0x104,        //transport handled the command, success
            MI_INI_HANDLED_ERROR = 0x105,        //transport handled the command, with error
            MI_INI_NOT_HANDLED = 0x106,        //transport did not handle the command
        };
    }
}




