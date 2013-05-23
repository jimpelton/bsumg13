// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-05-18                                                                      
// ******************************************************************************

#ifndef SimpleCapture2_h__
#define SimpleCapture2_h__

#include <Windows.h>

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include <midlib2.h>

#ifdef UG_EXPORT_SIMPLE_CAPTURE
#define UGEXPORT __declspec(dllexport)
#else
#define UGEXPORT
#endif

#define PIXELBITS  16
#define MAX_CAMS   2

typedef void (__stdcall *AttachCallback)(int camIdx);

struct ugCamera {
mi_camera_t     *pCamera; //=NULL;                     //current camera
int             camIdx;
int             camNm;
unsigned long   frameSize;                             //size of the frames we want to save
unsigned char   *pCameraBuff;                          //memory buffer for all the sensor images
unsigned char   *pGrabframeBuff;                       //grabFrame buffer
unsigned long   nWidth;                                //width of image (taken from command line or set to default below)
unsigned long   nHeight;                               //height of image (taken from command line or set to default below)
FILE            *imfile;                               //capture file
unsigned int    frame;
mi_u32          nSwizzleNeeded;
char            imagetypestr[32];
char            *iniFilePath;
};

mi_string       errorFileName;


extern "C" 
{
    UGEXPORT mi_u32 miDevCallBack(HWND, _mi_camera_t*, mi_u32);
    UGEXPORT int initMidLib2(int nCamsReq, void *hwnd, long attach_cb);
    UGEXPORT void setIniPath(char *);
    UGEXPORT int  openTransportIdx(int camIdx); 
    UGEXPORT unsigned char* doCaptureIdx(int camIdx, int *errval);
    UGEXPORT int getWavelengthIdx(int camIdx);
    UGEXPORT int stopTransportIdx(int camIdx);
    UGEXPORT unsigned long sensorBufferSizeIdx(int camIdx);
    UGEXPORT void printCameraInfo();
    UGEXPORT int setDeviceCallback(void * hwnd);
}
#endif /* SimpleCapture2_h__ */

