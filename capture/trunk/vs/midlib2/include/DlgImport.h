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
//      This software and any associated documentation are provided "AS IS" and                
//      without warranty of any kind.   APTINA IMAGING CORPORATION EXPRESSLY DISCLAIMS         
//      ALL WARRANTIES EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO, NONINFRINGEMENT       
//      OF THIRD PARTY RIGHTS, AND ANY IMPLIED WARRANTIES OF MERCHANTABILITY OR FITNESS        
//      FOR A PARTICULAR PURPOSE.  APTINA DOES NOT WARRANT THAT THE FUNCTIONS CONTAINED        
//      IN THIS SOFTWARE WILL MEET YOUR REQUIREMENTS, OR THAT THE OPERATION OF THIS SOFTWARE   
//      WILL BE UNINTERRUPTED OR ERROR-FREE.  FURTHERMORE, APTINA DOES NOT WARRANT OR          
//      MAKE ANY REPRESENTATIONS REGARDING THE USE OR THE RESULTS OF THE USE OF ANY            
//      ACCOMPANYING DOCUMENTATION IN TERMS OF ITS CORRECTNESS, ACCURACY, RELIABILITY,         
//      OR OTHERWISE.                                                                          
//*************************************************************************************/       
// DlgImport.h 
//

#ifndef _DLGIMPORT_H_
#define _DLGIMPORT_H_

//  API version number (integer)
#define PLUGIN_DLG_API      5

//  Functions in DevWare called from the DLL
typedef unsigned int  (CALLBACK *DEVW_ReadRegister)(const char *regname, const char *fieldname, int cached);
typedef unsigned int  (CALLBACK *DEVW_ReadRegisterAddr)(unsigned int nAddr, unsigned int nBitmask, int nAddrSpace, mi_addr_type nAddrType, int cached);
typedef void          (CALLBACK *DEVW_WriteRegister)(const char *regname, const char *fieldname, unsigned int value);
typedef void          (CALLBACK *DEVW_WriteRegisterAddr)(unsigned int nAddr, unsigned int nBitmask, int nAddrSpace, mi_addr_type nAddrType, unsigned int nValue);
typedef unsigned int  (CALLBACK *DEVW_PeekRegister)(unsigned int nBaseAddr, unsigned int nAddr, int nAddrSize, int nDataSize);
typedef void          (CALLBACK *DEVW_PokeRegister)(unsigned int nBaseAddr, unsigned int nAddr, unsigned int nValue, int nAddrSize, int nDataSize);
typedef void          (CALLBACK *DEVW_SyncChanges)(int sync);
typedef void          (CALLBACK *DEVW_Stop)(int stop);
typedef void          (CALLBACK *DEVW_MonitorRegister)(const char *regname, int id, HWND hwnd);
typedef int           (CALLBACK *DEVW_GetState)(const char *state);
typedef void          (CALLBACK *DEVW_SetState)(const char *state, int nValue);
typedef void          (CALLBACK *DEVW_PokeRegisters)(unsigned int nBaseAddr, unsigned int nAddr, int nCount, void *pValues, int nAddrSize, int nDataSize);
typedef void          (CALLBACK *DEVW_Pause)(int pause);
typedef void          (CALLBACK *DEVW_PeekRegisters)(unsigned int nBaseAddr, unsigned int nAddr, int nCount, void *pValues, int nAddrSize, int nDataSize);
typedef char *        (CALLBACK *DEVW_GetStateStr)(const char *state);
typedef void          (CALLBACK *DEVW_SetStateStr)(const char *state, const char *szValue);
typedef void          (CALLBACK *DEVW_LoadPreset)(const char *szFilename, const char *szPreset);
typedef int           (CALLBACK *DEVW_RunPython)(const char *szStatements);
typedef int           (CALLBACK *DEVW_GetOption)(const char *option, int nDefault);
typedef void          (CALLBACK *DEVW_SetOption)(const char *option, int nValue);
typedef void          (CALLBACK *DEVW_GetOptionStr)(const char *option, char *szValue, int nBufSize, const char *szDefault);
typedef void          (CALLBACK *DEVW_SetOptionStr)(const char *option, const char *szValue);
typedef void          (CALLBACK *DEVW_GetMouseSelection)(int* pnSelectType, int* pnStartX, int* pnStartY, int* pnEndX, int* pnEndY);
typedef void          (CALLBACK *DEVW_SetMouseSelection)(int nSelectType, int nStartX, int nStartY, int nEndX, int nEndY);
typedef void          (CALLBACK *DEVW_BeginAccessRegs)(void);
typedef void          (CALLBACK *DEVW_EndAccessRegs)(void);

typedef struct tag_devw_callbacks
{
    DEVW_ReadRegister       ReadRegister;
    DEVW_ReadRegisterAddr   ReadRegisterAddr;
    DEVW_WriteRegister      WriteRegister;
    DEVW_WriteRegisterAddr  WriteRegisterAddr;
    DEVW_PeekRegister       PeekRegister;
    DEVW_PokeRegister       PokeRegister;
    DEVW_SyncChanges        SyncChanges;
    DEVW_MonitorRegister    MonitorRegister;
    DEVW_Stop               Stop;
    DEVW_GetState           GetState;
    DEVW_SetState           SetState;
    DEVW_PokeRegisters      PokeRegisters;
    DEVW_Pause              Pause;
    DEVW_PeekRegisters      PeekRegisters;
    DEVW_GetStateStr        GetStateStr;
    DEVW_SetStateStr        SetStateStr;
    DEVW_LoadPreset         LoadPreset;
    DEVW_RunPython          RunPython;
    DEVW_GetOption          GetOption;
    DEVW_SetOption          SetOption;
    DEVW_GetOptionStr       GetOptionStr;
    DEVW_SetOptionStr       SetOptionStr;
    DEVW_GetMouseSelection  GetMouseSelection;
    DEVW_SetMouseSelection  SetMouseSelection;
    DEVW_BeginAccessRegs    BeginAccessRegs;
    DEVW_EndAccessRegs      EndAccessRegs;
}
DEVW_Callbacks;
#define WM_PLUGIN_HIDE (WM_APP+0x211)
#ifndef WM_MONITOR
#define WM_MONITOR (WM_APP+0x111)
#endif


//  Camera data for the DLL
typedef struct tag_devw_camera
{
    char *                  sensorName; 
    mi_sensor_types         sensorType;
    mi_u32                  fullWidth;
    mi_u32                  fullHeight;
    mi_u32                  sensorVersion;
    char *                  partNumber;
    mi_product_ids          productID;
    mi_u32                  productVersion;
    char *                  productName;
    mi_u32                  firmwareVersion;
    mi_camera_t *           pCamera;
}
DEVW_Camera;

#ifdef __cplusplus
class QWidget;
#endif

//  Functions in the DLL called from DevWare
#ifdef __cplusplus
extern "C"
{
#endif
typedef HWND (CALLBACK *DLLFN_OpenDialog)(
                DWORD           dwApiVersion,
                HWND            hwndParent,
                DEVW_Camera     *pCamera,
                DEVW_Callbacks  *pCallbacks);

#ifdef __cplusplus
typedef QWidget * (CALLBACK *DLLFN_OpenQtDialog)(
                DWORD           dwApiVersion,
                QWidget *       widgetParent,
                DEVW_Camera     *pCamera,
                DEVW_Callbacks  *pCallbacks);
#endif

typedef void (CALLBACK *DLLFN_CloseDialog)(void);

typedef void (CALLBACK *DLLFN_ImageData)(
                unsigned char *pImage,
                mi_image_types imageType,
                int           nWidth,
                int           nHeight,
                int           nStride);

typedef void (CALLBACK *DLLFN_GrabFrame)(
                unsigned char * pBuffer,
                int             nDataLength);
#ifdef __cplusplus
}
#endif
#endif
