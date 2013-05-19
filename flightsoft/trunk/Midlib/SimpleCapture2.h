
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
    UGEXPORT int initMidLib2(int nCamsReq);
    UGEXPORT void setIniPath(char *);
    UGEXPORT int  openTransport(ugCamera*); 
    UGEXPORT unsigned char* doCapture(int camIdx);
    UGEXPORT int getWavelength(int camIdx);
    UGEXPORT void stopTransport();
    UGEXPORT unsigned long sensorBufferSize(int camIdx);
    UGEXPORT void printCameraInfo();

}
#endif /* SimpleCapture2_h__ */
