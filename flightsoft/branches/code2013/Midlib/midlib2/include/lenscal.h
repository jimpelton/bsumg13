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

#ifndef _LENSCAL_H_
#define _LENSCAL_H_

#ifndef MI_MAX_CAMERAS
#pragma message("lenscal.h requires midlib2.h to be included ahead of it")
#endif


enum RawFormat
{
    RAW_UNKNOWN,
    RAW_BINARY,
    RAW_DECIMAL,
    RAW_HEX,
    RAW_ITX,
};


typedef struct _YCbCrParams
{
    size_t      size;
    int         nCoeffCtrl; // 0 = sRGB coeffs, 1 = CCIR-601 coeffs.
    //                        CbYCrY  YCbYCr etc.
    int         nCb_offset; //   0       1
    int         nY0_offset; //   1       0
    int         nCr_offset; //   2       3
    int         nY1_offset; //   3       2
    int         nLumaBlack; // Luma black, usu. 0 or 16
    int         nLumaWhite; // Luma white, usu. 255 or 235
    int         nChromaRange; // usu. 255 or 224
    int         nC_excess;  //  128 for YCbCr or 0 for YUV (signed chroma)
    int         nFlipMirror;
} YCbCrParams;

typedef struct _CalibParams
{
    size_t      size;
    int         nFalloff;//  Red channel or all if nFalloff2 is zero
    int         nIterations;
    int         bOuterBands;
    int         nPolyFitGridX;
    int         nPolyFitGridY;
    double      dMargin;
    double      dGuaranteeMinGain;
    int         nCoeffPrec;
    int         nLensRadius;  // Was nWideAngle - If the lens doesn't cover the whole array set the radius of the lens in pixels of a full size image
                              // 0 means use the whole array; 1 sets the radius to half the image width (for some backward compatibility with nWideAngle)
                              // When using this you might want to constrain the center coordinate with the following fields.
    int         nXcenter_min;
    int         nXcenter_max;
    int         nYcenter_min;
    int         nYcenter_max;
    int         nFalloff4[3];//  G1, G2, B
    int         nLumaOnly; //  Same correction to all Bayer channels
} CalibParams;

typedef enum {
    SOC_POS_LEFT    = 2,    //  A light
    SOC_POS_MIDDLE  = 3,
    SOC_POS_RIGHT   = 4,    //  D65
} SocPosition;

typedef struct _ApgaParams
{
    size_t      size;
    int         nFalloff;
    int         nSocColorTemp;
    int         nSocPosition;   //  SOC_POS_LEFT/MIDDLE/RIGHT
    int         nXcenter;       //  Register values
    int         nYcenter;
    int         nPolyFitGridX;
    int         nPolyFitGridY;
    int         nIterations;
    int         nLensRadius;  // was nWideAngle
} ApgaParams;

typedef struct _AutoPgaParams
{
    size_t      size;
    int         nFalloff;
    int         nSocColorTemp;
    int         nSocPosition;   //  SOC_POS_LEFT/MIDDLE/RIGHT
    int         nPolyFitGridX;
    int         nPolyFitGridY;
    double      dMargin;
    double      dGuaranteeMinGain;
    int         nCoeffPrec;
    int         nXcenter_min;
    int         nXcenter_max;
    int         nYcenter_min;
    int         nYcenter_max;
    int         nIterations;
    int         nLensRadius;
} AutoPgaParams;

typedef struct _GenericLscParams
{
    size_t      size;
    int         nXzones;
    int         nYzones;
    int         nGap;
    int         nCenterW;
    int         nCenterH;
} GenericLscParams;

enum RegFormat
{
    REGFMT_DEVWARE_INI,
    REGFMT_C_ARRAY,
    REGFMT_DEVWARE_INI_FIELDWR,
};

//  Data structure for in-memory register/value list
typedef struct _RegValue
{
    unsigned int    nPage;
    unsigned int    nReg;
    unsigned int    nValue;
} RegValue;

typedef struct _RegList
{
    int         count;
    RegValue    *pRegValues;
} RegList;


//      API Return Value Codes
enum
{
    LCRET_RECONCILE_ZEROTHRESH  = 3,//Warning: ZERO_THRESHOLD differs from basis setting
    LCRET_RECONCILE_NEGPEDESTAL = 2,//Warning: NEG_PEDESTAL differs from basis setting
    LCRET_RECONCILE_AUTOCORNER  = 1,//Warning: AUTO_CORNER differs from basis setting
    LCRET_OK                    = 0,
    LCRET_ERROR                 = -1,
    LCRET_IMAGESIZE             = -2,//Mismatch between image size and sensor
    LCRET_RECONCILE_X2FACTORS   = -3,//Can't reconcile with basis setting. X2_FACTORS bit > basis
    LCRET_RECONCILE_DDERIV_1    = -4,//Can't reconcile with basis setting. DIVISOR_DERIV_1_x < basis
    LCRET_RECONCILE_CORNER      = -5,//Can't reconcile with basis setting. Corner factor would overflow
    LCRET_MISSINGSDAT           = -6,//Can't find sensor data (.sdat) file for the sensor
};


#define POLYNOMIAL_GRID_MINIMUM_WIDTH  16
#define POLYNOMIAL_GRID_MINIMUM_HEIGHT 8


#ifdef LENSCAL_EXPORTS
#define LENSCAL_API  __declspec(dllexport)
#else
#define LENSCAL_API  __declspec(dllimport)
#endif
#define LENSCAL_DECL  __stdcall


#ifdef __cplusplus
extern "C" {
#endif

LENSCAL_API void * LENSCAL_DECL
lc_Create(mi_sensor_types sensorType, int nRev, int nPolynomialOrder);
LENSCAL_API void * LENSCAL_DECL
lc_Create2(const char *szSensorDataPath,
           mi_sensor_types sensorType, int nRev, int nPolynomialOrder);

LENSCAL_API void LENSCAL_DECL
lc_Destroy(void *lc);

LENSCAL_API short * LENSCAL_DECL
lc_LoadInputImageFile(void *lc, const char *szInFileName, int nBayerPattern,
                      int nBlack, int nCropLeft, int nCropTop);

LENSCAL_API short * LENSCAL_DECL
lc_LoadInputImageFile2(void *lc, const char *szInFileName, int nBayerPattern,
                       int nFlipMirror, int nBlack, int nCropLeft, int nCropTop);

LENSCAL_API short * LENSCAL_DECL
lc_SetInputImage(void *lc, short *pOriginal, int nWidth, int nHeight,
                 int nBpp, int nBayerPattern, int nBlack,
                 int nCropLeft, int nCropTop);

LENSCAL_API short * LENSCAL_DECL
lc_SetInputImage2(void *lc, short *pOriginal, int nWidth, int nHeight,
                  int nBpp, int nBayerPattern, int nFlipMirror, int nBlack,
                  int nCropLeft, int nCropTop);

LENSCAL_API short * LENSCAL_DECL
lc_LoadInputImageFileYcbcr(void *lc, const char *szInFileName,
                           YCbCrParams *ycbcrParams,
                           int nCropLeft, int nCropTop);

LENSCAL_API short * LENSCAL_DECL
lc_SetInputImageYcbcr(void *lc, unsigned char *pOriginal, int nWidth, int nHeight,
                      YCbCrParams *ycbcrParams,
                      int nCropLeft, int nCropTop);

LENSCAL_API int LENSCAL_DECL
lc_SetImageTransform(void *lc, float *pTrans, int nWidth, int nHeight);

LENSCAL_API int LENSCAL_DECL
lc_LoadInputIniFile(void *lc, const char *szInFileNameIni, const char *szSection);

LENSCAL_API int LENSCAL_DECL
lc_LoadBasisSetting(void *lc, const char *szBasisFileNameIni, const char *szSection);

LENSCAL_API int LENSCAL_DECL
lc_LoadBandsFile(void *lc, const char *szBandsFileName);

LENSCAL_API int LENSCAL_DECL
lc_FindSolution(void *lc, CalibParams *pCalibParams);

LENSCAL_API int LENSCAL_DECL
lc_ResetPga(void *lc);

LENSCAL_API int LENSCAL_DECL
lc_ResetApga(void *lc);

LENSCAL_API int LENSCAL_DECL
lc_FindApga(void *lc, ApgaParams *pApgaParams);

LENSCAL_API int LENSCAL_DECL
lc_ResetAutoPga(void *lc);

LENSCAL_API int LENSCAL_DECL
lc_FindAutoPga(void *lc, AutoPgaParams *pAutoPgaParams);

LENSCAL_API RegList * LENSCAL_DECL
lc_DumpRegList(void *lc);

LENSCAL_API int LENSCAL_DECL
lc_DumpRegFile(void *lc, const char *szOutFileName, enum RegFormat format,
               const char *szHeader, const char *szFooter);

LENSCAL_API int LENSCAL_DECL
lc_DumpRegFileAppend(void *lc, const char *szOutFileName, enum RegFormat format,
                     const char *szHeader, const char *szFooter);

LENSCAL_API short * LENSCAL_DECL
lc_SimulateCorrectedImage(void *lc, const char * szOutFileName,
                          enum RawFormat outFormat, int nBlack);

LENSCAL_API int LENSCAL_DECL
lc_CalculateCorrectedSbl(void *lc, double *pResults, int nLength);

LENSCAL_API int LENSCAL_DECL
lc_CalculateCorrectedAvgColor(void *lc, double *pResults, int nLength);

LENSCAL_API int LENSCAL_DECL
lc_CalculateGenericLsc(void *lc, GenericLscParams *params,
                       double *pResults /* nX * nY * 4 items; Gr, R, B, Gb; 0.0 - 1.0 scale*/,
                       int nLength); // experimental
LENSCAL_API int LENSCAL_DECL
lc_CalculateGenericCenter(void *lc, GenericLscParams *params,
                          double *pResults /* 4 items; Gr, R, B, Gb; 0.0 - 1.0 scale*/,
                          int nLength); // experimental

#ifdef __cplusplus
}
#endif

#endif
