unit Midlib2;
interface

//**************************************************************************************
// Copyright 2009 Aptina Imaging Corporation. All rights reserved.
//
//
// No permission to use, copy, modify, or distribute this software and/or
// its documentation for any purpose has been granted by Aptina Imaging Corporation.
// If any such permission has been granted ( by separate agreement ), it
// is required that the above copyright notice appear in all copies and
// that both that copyright notice and this permission notice appear in
// supporting documentation, and that the name of Aptina Imaging Corporation or any
// of its trademarks may not be used in advertising or publicity pertaining
// to distribution of the software without specific, written prior permission.
//
//
// This software and any associated documentation are provided “AS IS” and
// without warranty of any kind. APTINA IMAGING CORPORATION EXPRESSLY DISCLAIMS
// ALL WARRANTIES EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO, NONINFRINGEMENT
// OF THIRD PARTY RIGHTS, AND ANY IMPLIED WARRANTIES OF MERCHANTABILITY OR FITNESS
// FOR A PARTICULAR PURPOSE. APTINA DOES NOT WARRANT THAT THE FUNCTIONS CONTAINED
// IN THIS SOFTWARE WILL MEET YOUR REQUIREMENTS, OR THAT THE OPERATION OF THIS SOFTWARE
// WILL BE UNINTERRUPTED OR ERROR-FREE. FURTHERMORE, APTINA DOES NOT WARRANT OR
// MAKE ANY REPRESENTATIONS REGARDING THE USE OR THE RESULTS OF THE USE OF ANY
// ACCOMPANYING DOCUMENTATION IN TERMS OF ITS CORRECTNESS, ACCURACY, RELIABILITY,
// OR OTHERWISE.
//*************************************************************************************/

//Make sure that the data is packed w/ 8-byte alignment
//Also make sure that enums are treated as integers
{$Z4}
{$A+}

//***************************************************************************
//  Error and log types
//
//  MI_NO_ERROR_LOG    Error logging is turned off
//  MI_ERROR_SEVERE    Log Severe errors
//  MI ERROR_MINOR     Log Minor errors
//  MI_ALL_ERRORS      Logs all error messages (Severe and Minor)
//  MI_LOG             Logs General logging message
//  MI_LOG_SHIP        Log Serial I/O messages (SHIP) 
//  MI_LOG_USB         Log USB transactions 
//  MI_LOG_DEBUG       Log Debug messages
//*****************************************************************************/
const MI_NO_ERROR_LOG         =    0;
const MI_ERROR_SEVERE         =    1;
const MI_ERROR_MINOR          =    2;
const MI_ALL_ERRORS           =    (MI_ERROR_SEVERE + MI_ERROR_MINOR);
const MI_LOG                  =    4;
const MI_LOG_SHIP             =    8;
const MI_LOG_USB              =    16;
const MI_LOG_DEBUG            =    32;
const MI_ALL_LOGS             =    (MI_LOG + MI_LOG_SHIP + MI_LOG_USB + MI_LOG_DEBUG);
const MI_ERROR_LOG_ALL        =    (MI_ALL_ERRORS + MI_ALL_LOGS);

//****************************************************************************
//  MI_MAX_CAMERAS       maximum number of cameras supported
//  MI_MAX_REGS          maximum number of general registers supported
//  MI_APTINA_VID        Aptina Imaging Vendor ID for USB
//  MI_MICRON_VID        Micron Imaging Vendor ID for USB
//  MI_MICRON_PCI_VID    Micron Imaging Vendor ID for PCI
//  MI_MAX_STRING        maximum length of mi_string type
//****************************************************************************
     MI_MAX_CAMERAS         =       20;
     MI_MAX_REGS            =       400;
     MI_APTINA_VID			=		$20FB;
     MI_MICRON_VID          =       $0634;
     MI_MICRON_PCI_VID      =       $1344;
     MI_MAX_STRING          =       256;
          
//***************************************************************************
//  Available Camera Transports
//***************************************************************************
    MI_USB_BULK_TRANSPORT              = $0001;
    MI_CARDBUS_TRANSPORT               = $0002;
    MI_DLL_TRANSPORT                   = $0004;
    MI_CL_TRANSPORT                    = $0008;
    MI_BMP_TRANSPORT                   = $0010;
    MI_AVI_TRANSPORT                   = $0020;
    MI_RAW_TRANSPORT                   = $0040;
    MI_ALL_TRANSPORTS                  = $FFFF;
    MI_NUL_TRANSPORT                   = $10000; // not included in MI_ALL_TRANSPORTS

//**************************************************************************
//  Device Removal/Arrival Notification Flags
//***************************************************************************
    MI_DEVEVENT_REMOVAL    =  $00000001;
    MI_DEVEVENT_ARRIVAL    =  $00000002;
    MI_DEVEVENT_PRIMARY    =  $00000004;
    MI_DEVEVENT_OTHER      =  $00000008;

//***************************************************************************
// 
//  Type Definitions
//
//  The following types are used to maintain portability.
//***************************************************************************
type mi_u8  = Byte;
type mi_u16 = Word;
type mi_u32 = Longword;
type mi_s8  = Shortint;
type mi_s16 = Smallint;
type mi_s32 = Longint;
type mi_string = array[0..MI_MAX_STRING-1] of char;

type Pmi_u32 = ^mi_u32;
type Pmi_s32 = ^mi_s32;


//****************************************************************************
//  Enums
//                    - Function return codes
//    mi_image_types  - Supported image types
//    mi_product_ids  - Current camera boards supported
//    mi_sensor_types - Supported sensors
//    mi_chip_types   - Known companion chip types
//    mi_modes        - Modes which can be used with setMode/getMode routines
//    mi_unswizzle_modes - Modes for unswizzling, used with setMode/getMode
//    mi_sync_types   - Possible read/write register synchronization modes
//
//*****************************************************************************/

const
     MI_CAMERA_SUCCESS         =   $00; // Success value for midlib routines
     MI_CAMERA_ERROR           =   $01; // General failure for midlib routines
     
     MI_GRAB_FRAME_ERROR       =   $03; // General failure for grab frame routine
     MI_NOT_ENOUGH_DATA_ERROR  =   $04; // Grab frame failed to return enough data
     MI_EOF_MARKER_ERROR       =   $05; // EOF packet not found in grab frame dat
     MI_BUFFER_SIZE_ERROR      =   $06; // GrabFrame buffer is too small
     MI_SENSOR_NOT_INITIALIZED =   $09; // The sensor structure has not been initialized (call updateFrame)

     MI_SENSOR_FILE_PARSE_ERROR =  $07; // There was an error parsing the sdat file
     MI_SENSOR_DOES_NOT_MATCH   =  $08; // Cannot find sdat file which matches sensor
     MI_SENSOR_NOT_SUPPORTED    =  $0A; // The sensor is no longer supported

     MI_I2C_BIT_ERROR           =  $0B; // I2C bit error  
     MI_I2C_NACK_ERROR          =  $0C; // I2C NAC error
     MI_I2C_TIMEOUT             =  $0D; // I2C time out error
     MI_CAMERA_TIMEOUT          =  $0E;

     MI_CAMERA_NOT_SUPPORTED    =  $10; // The function call is not supported
     MI_PARSE_SUCCESS           =  $20; // Parsing was successful
     MI_DUPLICATE_DESC_ERROR    =  $21; // Duplicate unique descriptor was found
     MI_PARSE_FILE_ERROR        =  $22; // Unable to open sensor data file
     MI_PARSE_REG_ERROR         =  $23; // Error parsing the register descriptors
     MI_UKNOWN_SECTION_ERROR    =  $24; // Unknown Section found in sensor data file
     MI_CHIP_DESC_ERROR         =  $25; // Error parsing the chip descriptor section
     MI_PARSE_ADDSPACE_ERROR    =  $26; // Error parsing the address space section

     MI_INI_SUCCESS             = $100; // INI Preset is loaded successfully
     MI_INI_KEY_NOT_SUPPORTED   = $101; // Key is not supported - will be ignored
     MI_INI_LOAD_ERROR          = $102; // Error loading INI preset
     MI_INI_POLLREG_TIMEOUT     = $103; // Time out in POLLREG comman

type mi_image_types =(MI_UNKNOWN_IMAGE_TYPE, 
                     MI_BAYER_8,
                     MI_BAYER_10,
                     MI_BAYER_8_ZOOM,
                     MI_BAYER_10_ZOOM,
                     MI_YCBCR,
                     MI_RGB565,
                     MI_RGB555,
                     MI_RGB444X,
                     MI_RGBX444,
                     MI_RGB24,
                     MI_RGB32,
                     MI_BAYER_12,
                     MI_BAYER_S12,
					 MI_BAYER_S24,
                     MI_RGB48,
                     MI_JPEG,
                     MI_BAYER_STEREO,
                     MI_PNG,
                     MI_BGRG,
                     MI_YUV420,
                     MI_BAYER_14,
                     MI_BAYER_12_HDR,
                     MI_BAYER_14_HDR,
                     MI_BAYER_20,
                     MI_RGB332,
                     MI_M420,
                     MI_BAYER_10_IHDR,
                     MI_JPEG_SPEEDTAGS,
                     MI_BAYER_16,
                     MI_YCBCR_10,
                     MI_BAYER_6,
                     MI_JPEG_ROT,
                     MI_Y400,
                     MI_RGB555L,
                     MI_RGB555M
                     );

const
     MI_UNKNOWN_PRODUCT = 0;
     MI_BIGDOG    = $1002;
     MI_DEMO_1    = $1003;
     MI_DEMO_1A   = $1004;
     MI_WEBCAM    = $1006;
     MI_DEMO_2    = $1007;
     MI_DEV_2     = $1008;
     MI_MIGMATE   = $1009;
     MI_PCCAM     = $100A;
     MI_MIDES     = $100B;
     MI_MIDES_XL  = $100C;
     MI_DEMO_2X   = $100D;
     MI_DEMO_3    = $100E;
     MI_CLINK_1   = $5555;            // Camera Link product ID
     MI_CARDCAM_1 = $D100;
     
type mi_product_ids = Integer;

type mi_sensor_types = (MI_UNKNOWN_SENSOR, 
                        MI_0133,        //No longer supported
                        MI_0343,        //No longer supported
                        MI_0360,
                        MI_0366,
                        MI_0133_SOC,    //No longer supported
                        MI_0343_SOC,    //No longer supported
                        MI_1300,
                        MI_1310_SOC,
                        MI_2000,
                        MI_0360_SOC,
                        MI_1310,
                        MI_3100,
                        MI_0350,
                        MI_0366_SOC,
                        MI_2010,
                        MI_2010_SOCJ,
                        MI_0370,
                        MI_RESERVED_SENSOR0,
                        MI_SENSOR_X,    //Used for testing
                        MI_1320_SOC,
                        MI_1320,        //No longer supported
                        MI_5100,
                        MI_RESERVED_SENSOR1,
                        MI_0354_SOC,
                        MI_3120,
                        MI_RESERVED_SENSOR2,
                        MI_RESERVED_SENSOR3,
                        MI_RESERVED_SENSOR4,
                        MI_RESERVED_SENSOR5,
                        MI_RESERVED_SENSOR6,
                        MI_RESERVED_SENSOR7,
                        MI_RESERVED_SENSOR8,
                        MI_RESERVED_SENSOR9,
                        MI_RESERVED_SENSOR10,
                        MI_RESERVED_SENSOR11,
                        MI_BITMAP_SENSOR,
                        MI_RESERVED_SENSOR12,
                        MI_RESERVED_SENSOR13,
                        MI_RESERVED_SENSOR14,
                        MI_RESERVED_SENSOR15,
                        MI_RESERVED_SENSOR16,
                        MI_RESERVED_SENSOR17,
                        MI_RESERVED_SENSOR18,
                        MI_RESERVED_SENSOR19,
                        MI_RESERVED_SENSOR20,
                        MI_RESERVED_SENSOR21,
                        MI_RESERVED_SENSOR22,
                        MI_RESERVED_SENSOR23,
                        MI_RESERVED_SENSOR24,
                        MI_RESERVED_SENSOR25,
                        MI_RESERVED_SENSOR26,
                        MI_RESERVED_SENSOR27,
                        MI_RESERVED_SENSOR28,
                        MI_RESERVED_SENSOR29,
                        MI_RESERVED_SENSOR30,
                        MI_RESERVED_SENSOR31,
                        MI_RESERVED_SENSOR32,
                        MI_RESERVED_SENSOR33,
                        MI_RESERVED_SENSOR34,
                        MI_RESERVED_SENSOR35,
                        MI_RESERVED_SENSOR36,
                        MI_RESERVED_SENSOR37,
                        MI_8131,
                        MI_1000GS,
                        MI_1000ERS,
                        MI_RESERVED_SENSOR38,
                        MI_RESERVED_SENSOR39,
                        MI_RESERVED_SENSOR40,
                        MI_RESERVED_SENSOR41,
                        MI_RESERVED_SENSOR42,
                        MI_RESERVED_SENSOR43,
                        MI_RESERVED_SENSOR44,
                        MI_RESERVED_SENSOR45,
                        MI_2031_SOC,
                        MI_RESERVED_SENSOR46,
                        MI_RESERVED_SENSOR47,
                        MI_5131E,
                        MI_5131E_PLCC,
                        MI_RESERVED_SENSOR48,
                        MI_RESERVED_SENSOR49,
                        MI_RESERVED_SENSOR50,
                        MI_RESERVED_SENSOR51,
                        MI_RESERVED_SENSOR52,
                        MI_RESERVED_SENSOR53,
                        MI_51HD_PLUS,
                        MI_RESERVED_SENSOR54,
                        MI_RESERVED_SENSOR55,
                        MI_RESERVED_SENSOR56,
                        MI_RESERVED_SENSOR57,
                        MI_RESERVED_SENSOR58,
                        MI_RESERVED_SENSOR59,
                        MI_RESERVED_SENSOR60,
                        MI_RESERVED_SENSOR61,
                        MI_RESERVED_SENSOR62,
                        MI_RESERVED_SENSOR63,
                        MI_RESERVED_SENSOR64,
                        MI_RESERVED_SENSOR65,
                        MI_RESERVED_SENSOR66,
                        MI_RESERVED_SENSOR67,
                        MI_RESERVED_SENSOR68,
                        MI_RESERVED_SENSOR69,
                        MI_RESERVED_SENSOR70,
                        MI_RESERVED_SENSOR71,
                        MI_RESERVED_SENSOR72,
                        MI_RESERVED_SENSOR73,
                        MI_RESERVED_SENSOR74,
                        MI_RESERVED_SENSOR75,
                        MI_RESERVED_SENSOR76,
                        MI_RESERVED_SENSOR77,
                        MI_RESERVED_SENSOR78,
                        MI_RESERVED_SENSOR79,
						MI_RESERVED_SENSOR80,
						MI_RESERVED_SENSOR81,
						MI_RESERVED_SENSOR82,
						MI_RESERVED_SENSOR83,
						MI_RESERVED_SENSOR84
                        );
//  Sensor names no longer reserved
const
     MI_2020_SOC  = MI_RESERVED_SENSOR1;
     MI_2020      = MI_RESERVED_SENSOR3;
     MI_0350_ST   = MI_RESERVED_SENSOR4;   //Stereo Test board
     MI_8130      = MI_RESERVED_SENSOR9;
     MI_1325      = MI_RESERVED_SENSOR10;
     MI_0380_SOC  = MI_RESERVED_SENSOR13;
     MI_1330_SOC  = MI_RESERVED_SENSOR14;
     MI_1600      = MI_RESERVED_SENSOR15;
     MI_0380      = MI_RESERVED_SENSOR16;
     MI_3130      = MI_RESERVED_SENSOR17;
     MI_5130      = MI_RESERVED_SENSOR18;
     MI_RAINBOW2  = MI_RESERVED_SENSOR19;
     MI_3130_SOC  = MI_RESERVED_SENSOR21;
     MI_2030_SOC  = MI_RESERVED_SENSOR22;
     MI_3125      = MI_RESERVED_SENSOR23;
     MI_0351      = MI_RESERVED_SENSOR24;
     MI_9130      = MI_RESERVED_SENSOR25;
     MI_5135      = MI_RESERVED_SENSOR26;
     MI_2025      = MI_RESERVED_SENSOR27;
     MI_0351_ST   = MI_RESERVED_SENSOR28;   //Stereo Test board
     MI_0356_SOC  = MI_RESERVED_SENSOR29;
     MI_5140_SOC  = MI_RESERVED_SENSOR30;
     MI_3132_SOC  = MI_RESERVED_SENSOR31;
     MI_10030     = MI_RESERVED_SENSOR32;
     MI_5131      = MI_RESERVED_SENSOR34;

type mi_modes      = (MI_ERROR_CHECK_MODE,
                      MI_REG_ADDR_SIZE,
                      MI_REG_DATA_SIZE,
                      MI_USE_REG_CACHE,
                      
                      MI_SW_UNSWIZZLE_MODE,
                      MI_UNSWIZZLE_MODE,
                      MI_SW_UNSWIZZLE_DEFAULT,
                      MI_DATA_IS_SWIZZLED,
                      
                      _MI_READ_SYNC,
                      _MI_WRITE_SYNC,
                      _MI_CONTINUOUS_READ,
                      
                      MI_ERRORLOG_LEVEL,
                      
                      MI_SPOOF_SIZE,
                      MI_SPOOF_SUPPORTED,
                      MI_HW_BUFFERING,
                      MI_OUTPUT_CLOCK_FREQ,
                      MI_ALLOW_FAR_ACCESS,
                      MI_PIXCLK_POLARITY,
                      MI_SENSOR_POWER,
                      MI_SENSOR_RESET,
                      
                      MI_DIRECT_VAR_ACCESS,
                      MI_XDMA_LOGICAL,
                      MI_XDMA_PHY_A15,
                      MI_XDMA_PHY_REGION,
                      MI_HW_FRAME_COUNT,
                      MI_HIDY,
                      MI_COMPRESSED_LENGTH,
                      MI_SENSOR_SHUTDOWN,
                      MI_XDMA_ADV_BASE,
                      MI_PIXCLK_FREQ,
                      MI_SIMUL_REG_FRAMEGRAB,
                      MI_DETECT_FRAME_SIZE,
                      MI_BITS_PER_CLOCK,
                      MI_CLOCKS_PER_PIXEL,
                      MI_RX_TYPE,              //  mi_rx_types
                      MI_RX_LANES,
                      MI_RX_BIT_DEPTH,
                      MI_RX_MODE,              //  mi_rx_modes
                      MI_RX_CLASS,
                      MI_RX_SYNC_CODE,
                      MI_RX_EMBEDDED_DATA,
                      MI_RX_VIRTUAL_CHANNEL,
                      MI_RX_MSB_FIRST,
                      MI_HDMI_MODE,
                      MI_EIS,  // electronic image stabilization (ICP-HD + HDMI Demo)
                      MI_MEM_CAPTURE,           // set number of frames to capture
                      MI_MEM_CAPTURE_PROGRESS,  // frames stored do far, read-only
                      MI_MEM_CAPTURE_MB,        // available RAM in MB, read-only
                      MI_STEREO_MODE,           //  mi_stereo_modes
                      MI_HW_FRAME_COUNT_MASK,
                      MI_GRABFRAME_TIMEOUT,    //timeout value per frame in grabFrame call
                      MI_HW_FRAME_TIME,
                      MI_LSB_ALIGNED,          //  parallel port data is LSB-aligned
                      MI_NACK_RETRIES,         //  number of times to retry after I2C NACK
                      MI_RX_CCIR656,           //  use CCIR-656 embedded sync codes
                      MI_RX_INTERLACED,         //  incoming stream is interlaced
                      MI_MEM_CAPTURE_CYCLE,    // linear, circular
                      MI_TRIGGER_HIGH_WIDTH,   // trigger high width in clock.
                      MI_TRIGGER_LOW_WIDTH,   // trigger low width in clock.
                      MI_TRIGGER, // trigger modes, 0 : No trigger, 1: single shot, 2: continuous.
                      MI_OUTPUT_PORT, // Port for image data out, 0: demo3, 1: CameraLink
                      MI_PIXEL_PACK,           //  Pack 10- or 12-bit pixel data
		      MI_PARITY
);

//for backwards compatibility
const MI_SWIZZLE_MODE = MI_SW_UNSWIZZLE_MODE; 
      MI_SWIZZLE_DEFAULT = MI_SW_UNSWIZZLE_DEFAULT;
      
type mi_unswizzle_modes = (MI_NO_UNSWIZZLE,
                      MI_HW_UNSWIZZLE,
                      MI_SW_UNSWIZZLE,
                      MI_ANY_UNSWIZZLE);

type mi_sync_types =(MI_SYNC_ASAP,
                     MI_VERT_BLANK,
                     MI_NOT_VERT_BLANK);

//****************************************************************************
//  Type: mi_bitfield_t
// 
//  This structure is used to hold register-bitfield description information
//  parsed from the sensor data files.  
//
//  id                bitfield descriptor name (register unique)
//  bitmask           bitmask describing valid bits
//  rw                1 if read/write; 0 if readonly
//  desc              description 
//  detail            additional details describing field
//*****************************************************************************/
type mi_bitfield_t = record
    id      : mi_string;
    bitmask : mi_u32;
    rw      : mi_s32;
    desc    : mi_string;
    detail  : mi_string;
end;
type Pmi_bitfield_t = ^mi_bitfield_t;

type mi_addr_type =(MI_REG_ADDR,
                    MI_MCU_ADDR,
                    MI_SFR_ADDR,
                    MI_IND_ADDR,
                    MI_FAR1_REG_ADDR, //  Registers on 1st sensor on far bus
                    MI_FAR1_MCU_ADDR, //  MCU driver variable on 1st sensor on far bus
                    MI_FAR1_SFR_ADDR, //  SFR on 1st sensor on far bus
                    MI_FAR2_REG_ADDR, //  Registers on 2nd sensor on far bus
                    MI_FAR2_MCU_ADDR, //  MCU driver variable on 2nd sensor on far bus
                    MI_FAR2_SFR_ADDR  //  SFR on 2nd sensor on far bus
                    );

//****************************************************************************
//  Type: mi_addr_space_val_t
// 
//  This structure is used to describe a value for the address space selector.  
//  This information is filled in when a sensor data file is parsed
//
//  ID     unique ID used in .sdat file - example "CORE"
//  name   name of the address space
//  val    value to set the address space selector to, or firmware driver
//  type   type of address (simple register, firmware variable, etc.)
//*****************************************************************************/
type mi_addr_space_val_t = record
    ID   : mi_string;
    name : mi_string;
    val  : mi_u32;
    addrType : mi_addr_type;
    far_base      : mi_u32;
    far_addr_size : mi_u32;
    far_data_size : mi_u32;
end;

//****************************************************************************
//  Type: mi_reg_data_t
//
//  This structure is used to hold all of the register bitmap information
//  stored in the sensor data files.  This structure can hold information
//  about an entire register or a particular bitfield of the register.
//
//    unique_desc   unique register descriptor name
//    reg_addr;     register address
//    reg_space;    addr space of register (superseded by addr_space)
//    bitmask;      bimask describing valid bits
//    default_val;  default value for bitfield
//    rw;           1 if read/write; 0 if readonly
//    reg_desc;     description of register
//    detail;       additional details describing field
//    num_bitfields the number of bitfields known for this register
//    bitfield      pointer to array of bitfields
//    addr_span     Number of addresses register covers
//    addr_space    Pointer to address space structure
//*****************************************************************************/
type mi_reg_data_t = record
    unique_desc   : mi_string;
    reg_addr      : mi_u32;
    reg_space     : mi_u32;
    bitmask       : mi_u32;
    default_val   : mi_u32;
    rw            : mi_s32;
    reg_desc      : mi_string;
    detail        : mi_string;
    num_bitfields : mi_s32;
    bitfield      : ^mi_bitfield_t;
    addr_span     : mi_s32;
    addr_space    : ^mi_addr_space_val_t;
end;
type Pmi_reg_data_t = ^mi_reg_data_t;

//****************************************************************************
//  Type: mi_addr_space_t
// 
//  This structure is used to describe the address space register for this sensor.
//  It is filled in when a sensor data file is parsed only for a sensor which has
//  an address space selector register.
//
//  regAddr           register address for the address space selector
//  num_vals          number of possible address spaces available
//  addr_space_val    array of possible address space values
//*****************************************************************************/
type mi_addr_space_t = record
    reg_addr        : mi_u32;
    num_vals        : mi_s32;
    addr_space_val  : ^mi_addr_space_val_t;
    far1_reg_addr   : mi_u32;
    far2_reg_addr   : mi_u32;
end;

//****************************************************************************
//  Type: mi_long_desc_t
// 
//  This structure is used to hold the "long description" of a register or
//  bitfield.
//
//  regName           register name
//  bitfieldName      bitfield name if this is a bitfield
//  longDesc          long description, UTF-8
//*****************************************************************************/
type mi_long_desc_t = record
    regName       : ^char;
    bitfieldName  : ^char;
    longDesc	  : ^char;
end;

//****************************************************************************
//  Type: mi_sensor_t
//
//  This structure is used to hold all of the pertinent information about
//  a supported sensor.
//
//  sensorName           Name of the sensor
//  sensorType           the sensor type (from mi_sensor_types)
//  fullWidth            full sensor image width
//  fullHeight           full sensor image height
//  width                current sensor image width
//  height               current sensor image height
//  zoomFactor           the current zoom factor (default 1, may be 2 or 4)
//  pixelBytes           number of bytes per pixel
//  pixelBits            number bits per pixel 
//  bufferSize           minimum size of the raw buffer used to get frame
//  imageType            the raw image type (from mi_image_types)
//  shipAddr             base SHIP address for sensor
//  reg_addr_size        Register address size 8/16
//  reg_data_size        Register data size 8/16
//  num_regs             number of registers
//  regs                 array of registers
//  addr_space           address spaces info (or NULL for Bayer Sensor)
//  sensorFileName       filename of .sdat file being used
//  sensorVersion        version number of sensor (default 1)
//  partNumber           Micron MT9 part number
//  versionName          name of version of part, for example Rev0
//
//*****************************************************************************/
type Pmi_sensor_t = ^mi_sensor_t;
  mi_sensor_t = record
    sensorName     : mi_string;
    sensorType     : mi_sensor_types;
    fullWidth      : mi_u32;
    fullHeight     : mi_u32;
    width          : mi_u32;
    height         : mi_u32;
    zoomFactor     : mi_u32;
    pixelBytes     : mi_u32;
    pixelBits      : mi_u32;
    bufferSize     : mi_u32;
    imageType      : mi_image_types;
    shipAddr       : mi_u32;
    reg_addr_size  : mi_s32;
    reg_data_size  : mi_s32;
    num_regs       : mi_s32;
    regs           : ^mi_reg_data_t;
    addr_space     : ^mi_addr_space_t;
    sensorFileName : mi_string;
    sensorVersion  : mi_u32;
    partNumber     : mi_string;
    versionName    : mi_string;
    far1_sensor    : Pmi_sensor_t;
    far2_sensor    : Pmi_sensor_t;
    num_long_desc  : mi_s32;
    long_desc      : ^mi_long_desc_t;
end;

//****************************************************************************
//  Type: mi_chip_t
//
//  This structure is used to hold all of the pertinent information about
//  a chip.
//
//  chipName         Name of the companion chip
//  chipType         the companion chip type (from mi_chip_types)
//  baseAddr         base I2C address for chip
//  serial_addr_size    Register address size 8/16bit
//  serial_data_size    Register data size 8/16bit 
//  num_regs         number of registers on companion chip
//  regs             array of num_regs registers on chip
//
//*****************************************************************************/
type mi_chip_t = record
    chipName    : mi_string;
    baseAddr    : mi_u32;
    serial_addr_size: mi_s32;
    serial_data_size: mi_s32;
    num_regs    : mi_s32;
    regs        : ^mi_reg_data_t;
end;

//****************************************************************************
//  Type: mi_frame_data_t
// 
//  This structure is used to hold all of the information about received for
//  a grabFrame.
//
//  frameNumber     -  The frame number of the last frame grabbed        
//  bytesRequested  -  The number of bytes requested for the last frame grabbed
//  bytesReturned   -  The number of bytes returned for the last frame grabbed      
//  numRegsReturned -  The number of register returned with the last frame grabbed      
//  regValsReturned -  The registers returned with the last frame grabbed      
//  imageBytesReturned - Then number of bytes of image data for the last frame grabbed
//
//****************************************************************************
type mi_frame_data_t = record
    frameNumber    : mi_u32;
    bytesRequested : mi_u32;
    bytesReturned  : mi_u32;
    numRegsReturned : mi_u32;
    regValsReturned : array[0..MI_MAX_REGS-3] of mi_u32;
    imageBytesReturned: mi_u32;
end;

//****************************************************************************
//  Type: _mi_camera_t
//
//  This structure is used to hold all of the pertinent information about
//  a camera.  
//
//  productID              Product ID for board
//  productVersion         Version # of the product
//  productName            Name of product
//  firmwareVersion        Version of the firmware on board
//  transportName          Name of the transport
//  transportType          Type of the transport
//  context                This is the camera specific context
//  sensor                 Sensor that is attached to the device
//  num_chips              Number of companion chips on board
//  chip                   The companion chip data
//
//  The following are function pointers for a specific transport
//  startTransport        - Starts a transport
//  stopTransport         - Stops a transport
//  readSensorRegisters   - read a sequence of sensor registers
//  writeSensorRegisters  - write a sequence of sensor register values
//  readSensorRegList     - read a non sequential list of sensor registers 
//  readRegister          - Reads a register
//  writeRegister         - Writes to a register
//  readRegisters         - read a sequence of registers
//  writeRegisters        - write a sequence of register values
//  grabFrame             - Returns an image frame
//  getFrameData          - Returns additional information about last grabbed frame
//  updateFrameSize       - Set frame data size given new width, height and bits per clock
//  updateBufferSize      - Set bufferSize given new rawBufferSize
//  setMode               - Sets one of the mi_modes 
//  getMode               - Gets the value of one of the mi_modes
//  initTransport         - Used to initialize the transport if .sdat file not used in openCameras
//*****************************************************************************/
type mi_camera_t = record
    productID      : mi_product_ids;
    productVersion : mi_u32;
    productName    : mi_string;
    firmwareVersion: mi_u32;
    transportName  : mi_string;
    transportType  : mi_u32;
    context        : Pointer;
    sensor         : Pmi_sensor_t;
    num_chips      : mi_s32;
    chip           : ^mi_chip_t;
    int_dev_functions: Pointer;
    startTransport       : function(pCamera : Pointer) : mi_s32; cdecl;
    stopTransport        : function(pCamera : Pointer) : mi_s32; cdecl;
    readSensorRegisters  : function(pCamera : Pointer; addrSpace: mi_u32; regAddr : mi_u32; numRegs : mi_u32; vals : Pointer): mi_s32; cdecl;
    writeSensorRegisters : function(pCamera : Pointer; addrSpace: mi_u32; regAddr : mi_u32; numRegs : mi_u32; vals : Pointer): mi_s32; cdecl;
    readSensorRegList    : function(pCamera : Pointer; numRegs: mi_u32; addrSpaces, regAddrs, vals : Pointer): mi_s32; cdecl;
    readRegister         : function(pCamera : Pointer; shipAddr,regAddr : mi_u32; val : Pointer) : mi_s32; cdecl;
    writeRegister        : function(pCamera : Pointer; shipAddr,regAddr : mi_u32; val : mi_u32) : mi_s32; cdecl;
    readRegisters        : function(pCamera : Pointer; shipAddr: mi_u32; regAddr : mi_u32; numRegs : mi_u32; vals : Pointer): mi_s32; cdecl;
    writeRegisters       : function(pCamera : Pointer; shipAddr: mi_u32; regAddr : mi_u32; numRegs : mi_u32; vals : Pointer): mi_s32; cdecl;
    grabFrame            : function(pCamera : Pointer; pInBuffer : Pointer; bufferSize: mi_u32) : mi_s32; cdecl;
    getFrameData         : function(pCamera : Pointer; frameData : Pointer): mi_s32; cdecl;
    updateFrameSize      : function(pCamera : Pointer; width, height: mi_u32; nBitsPerClock, nClocksPerPixel: mi_s32) : mi_s32; cdecl;
    updateBufferSize     : function(pCamera : Pointer; rawBufferSize: mi_u32) : mi_s32; cdecl;
    setMode              : function(pCamera : Pointer; mode, val: mi_u32) : mi_s32; cdecl;
    getMode              : function(pCamera : Pointer; mode, val: Pointer) : mi_s32; cdecl;
    initTransport        : function(pCamera : Pointer; bitsPerClock, clocksPerPixel, polarity, pixelOffset, noFWCalls: mi_s32): mi_s32; cdecl;
end;

type Pmi_camera_t = ^mi_camera_t;
type Pmi_addr_space_val_t = ^mi_addr_space_val_t;
type mi_camera_array = array[0..MI_MAX_CAMERAS-1] of Pmi_camera_t;
type charPtr = ^char;

//****************************************************************************
//  Exported Functions
//
//  mi_OpenCameras   - used to open all currently available camera transports
//  mi_OpenCameras2  - Extended version of mi_OpenCameras()
//  mi_CloseCameras  - closes all open camera transports
//  mi_CloseCameras2 - closes a list of camera transports
//
//  mi_ExistsBitfield     - determine if the registerName and bitfieldName exist in the sensor register list
//  mi_ExistsRegister     - determine if the registerName exists in the sensor register list
//  mi_FindBitfield       - return a pointer to the bitfield with "bitfieldName" within "registerName" in the sensor register list
//  mi_FindRegister       - return a pointer to the register with "registerName" in the sensor register list
//  mi_GetImageTypeStr    - Given a image type return a string with that name
//  mi_InvalidateRegCache - Invalidates the entire register cache
//  mi_IsBayer            - returns true if the current image type is a Bayer format
//  mi_IsSOC              - returns true if the sensor is considered an SOC sensor
//  mi_ParseSensorFile    - parse a given .sdat file into a mi_sensor_t structure
//  mi_ReadSensorReg      - read the register or variable using pointer to a register
//  mi_WriteSensorReg     - writes register or variable using pointer to a register
//  mi_ReadSensorRegStr   - read the register or variable using "register name" and "bitfield name" or NULL
//  mi_WriteSensorRegStr  - writes register or variable using "register name" and "bitfield name" or NULL
//  mi_LoadSectionINI     - used to load  a section from ini file. return MI_SECTION_SUCCESS: succeed; else error.
//  mi_ReadVars           - reads (sequence and continuous) variables using pointer to a register
//*****************************************************************************/
function  mi_OpenCameras(pCameras : mi_camera_array; nNumCameras : Pmi_s32; sensor_dir : mi_string) : mi_s32; cdecl; external 'midlib2.dll';
function  mi_OpenCameras2(pCameras : mi_camera_array; nNumCameras : Pmi_s32; sensor_dir : mi_string; transportType: mi_u32; dllName: mi_string) : mi_s32; cdecl; external 'midlib2.dll';
procedure mi_CloseCameras(); cdecl; external 'midlib2.dll';
procedure mi_CloseCameras2(pCameras : mi_camera_array; nNumCameras : mi_s32); cdecl; external 'midlib2.dll';
function  mi_Home(): charPtr; cdecl; external 'midlib2.dll';
function  mi_SensorData(): charPtr; cdecl; external 'midlib2.dll';

function  mi_ExistsBitfield(pCamera : Pmi_camera_t;  registerName, bitfieldName: mi_string): mi_s32; cdecl; external 'midlib2.dll';
function  mi_ExistsRegister(pCamera : Pmi_camera_t;  registerName: mi_string): mi_s32; cdecl; external 'midlib2.dll';
function  mi_FindBitfield(pCamera : Pmi_camera_t; registerName, bitfieldName: mi_string ): Pmi_bitfield_t; cdecl; external 'midlib2.dll';
function  mi_FindRegister(pCamera : Pmi_camera_t; registerName: mi_string): Pmi_reg_data_t; cdecl; external 'midlib2.dll';
function  mi_FindRegisterAddr(pCamera : Pmi_camera_t; regAddr : mi_u32; addrSpace : mi_u32; addrType : mi_addr_type): Pmi_reg_data_t; cdecl; external 'midlib2.dll';
function  mi_CurrentAddrSpace(pCamera : Pmi_camera_t): Pmi_addr_space_val_t ; cdecl; external 'midlib2.dll';

procedure mi_GetImageTypeStr(image_type: mi_image_types; pImageStr: mi_string); cdecl; external 'midlib2.dll';
procedure mi_InvalidateRegCache(pCamera : Pmi_camera_t); cdecl; external 'midlib2.dll';
function  mi_IsBayer(pCamera : Pmi_camera_t): mi_s32; cdecl; external 'midlib2.dll';
function  mi_IsSOC(pCamera : Pmi_camera_t): mi_s32; cdecl; external 'midlib2.dll';

function  mi_ParseSensorFile(pCamera : Pmi_camera_t; fileName: mi_string; sensor_data: Pmi_sensor_t):mi_s32; cdecl; external 'midlib2.dll';
function  mi_ParseChipFile(pCamera : Pmi_camera_t; fileName: mi_string;  chip_data: Pointer):mi_s32; cdecl; external 'midlib2.dll';
function  mi_LoadINIPreset(pCamera : Pmi_camera_t; iniFileName: mi_string; presetName : mi_string): mi_s32; cdecl; external 'midlib2.dll';

function  mi_ReadSensorReg(pCamera : Pmi_camera_t; pReg, val: Pointer):mi_s32; cdecl; external 'midlib2.dll';
function  mi_WriteSensorReg(pCamera : Pmi_camera_t; pReg: Pointer; val: mi_u32):mi_s32; cdecl; external 'midlib2.dll';
function  mi_ReadSensorRegAddr(pCamera : Pmi_camera_t; addrType : mi_addr_type; addrSpace: mi_u32; addr: mi_u32; is8: mi_s32; value: Pointer):mi_s32; cdecl; external 'midlib2.dll';
function  mi_WriteSensorRegAddr(pCamera : Pmi_camera_t; addrType : mi_addr_type; addrSpace: mi_u32; addr: mi_u32; is8: mi_s32; value: mi_u32):mi_s32; cdecl; external 'midlib2.dll';
function  mi_ReadSensorRegStr(pCamera : Pmi_camera_t; registerName, bitfieldName: mi_string; val: Pointer):mi_s32; cdecl; external 'midlib2.dll';
function  mi_WriteSensorRegStr(pCamera : Pmi_camera_t; registerName, bitfieldName: mi_string; val: mi_u32):mi_s32; cdecl; external 'midlib2.dll';

procedure mi_OpenErrorLog(error_log_level: mi_s32; baseFileName: mi_string); cdecl; external 'midlib2.dll';
procedure mi_CloseErrorLog(); cdecl; external 'midlib2.dll';
procedure mi_GetErrorLogFileName(fileName : mi_string); cdecl; external 'midlib2.dll';

implementation

end.
