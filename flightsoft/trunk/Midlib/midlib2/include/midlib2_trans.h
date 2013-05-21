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
// This software and any associated documentation are provided "AS IS" and
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


#ifndef __MIDLIB2_TRANS_H__   // [
#define __MIDLIB2_TRANS_H__


#ifdef __cplusplus
extern "C" {
#endif

    
typedef enum {MI_CONTEXT_BITS_PER_CLOCK = 1, // deprecated: use helper->setMode(...MI_BITS_PER_CLOCK...)
              MI_CONTEXT_CLOCKS_PER_PIXEL, // deprecated: use helper->setMode(...MI_CLOCKS_PER_PIXEL...)
              MI_CONTEXT_PIXCLK_POLARITY, // deprecated: use helper->setMode(...MI_PIXCLK_POLARITY...)
              MI_CONTEXT_PRIVATE_DATA,
              MI_CONTEXT_APP_HWND,
              MI_CONTEXT_DRIVER_INFO,
} mi_context_fields;

typedef struct
{
    mi_s32      (*getSensor)(mi_camera_t *pCamera, const char *sensor_dir_file);
    void        (*updateImageType)(mi_camera_t *pCamera);
    mi_s32      (*errorLog)(mi_s32 errorCode, mi_s32 errorLevel, const char *logMsg, const char *szSource, const char *szFunc, mi_u32 nLine);
    void        (*log)(mi_s32 logType, const char *logMsg, const char *szSource, const char *szFunc, mi_u32 nLine);
    mi_s32      (*setMode)(mi_camera_t *pCamera, mi_modes mode, mi_u32 val);
    mi_s32      (*getMode)(mi_camera_t *pCamera, mi_modes mode, mi_u32 *val);
    mi_s32      (*setContext)(mi_camera_t *pCamera, mi_context_fields field, mi_intptr val);
    mi_s32      (*getContext)(mi_camera_t *pCamera, mi_context_fields field, mi_intptr *val);
    void        (*updateFrameSize)(mi_camera_t *pCamera, mi_u32 nWidth, mi_u32 nHeight, mi_s32 nBitsPerClock, mi_s32 nClocksPerPixel);
    void        (*unswizzleBuffer)(mi_camera_t* pCamera, mi_u8 *pInBuffer);
    void        (*mergeDeepBayer)(mi_camera_t* pCamera, mi_u8 *pInBuffer);
    mi_u32      (*readReg)(mi_camera_t *pCamera, const char *szRegister, const char *szBitfield, mi_s32 bCached);
    mi_s32      (*getBoardConfig)(mi_camera_t *pCamera, const char *sensor_dir_file);
    void (*_reserved5)(void);
    void (*_reserved6)(void);
    void (*_reserved7)(void);
}  mi_transport_helpers_t;


#ifdef __cplusplus
}
#endif

#endif //__MIDLIB2_TRANS_H__ ]
