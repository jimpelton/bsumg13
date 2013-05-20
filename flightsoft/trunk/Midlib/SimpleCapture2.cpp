

#include "SimpleCapture2.h"
#include <stdio.h>

#include <midlib2.h>

int g_cameraIdx = -1;         //< this camera's index into gCameras.
int g_nextFrameIdx = 0;      //< unused.
int g_camNM = 0;             //< This camera's filter wavelength.

mi_s32 g_cams_found = 0;

ugCamera g_cameras[MAX_CAMS];
mi_camera_t *g_mi_cameras[MAX_CAMS];

int g_isMidLibInit = 0;

int mallocate(ugCamera *);
unsigned char * _doCapture(ugCamera *);

int initMidLib2(int nCamsReq)
{
    if (g_isMidLibInit) return 0;

    if (nCamsReq <= 0) {
        printf("You requested %d cams!" \
            "That is not a good number of cams to request!\n", 
            nCamsReq);
        return 1;
    }

    if (nCamsReq > MAX_CAMS) {
        return 1;
    }

    //memset(g_cameras, 0, sizeof(ugCamera)*MAX_CAMS);

    mi_s32 errval = mi_OpenCameras(g_mi_cameras, &g_cams_found, mi_SensorData());
    if ( errval != MI_CAMERA_SUCCESS ) {
        return errval;
    }
    printf("Found %d cameras.", g_cams_found);

    for (int i = 0; i < g_cams_found; ++i)
    {
        g_cameras[i].camIdx  = i;
        g_cameras[i].pCamera = g_mi_cameras[i];   

        errval = openTransport(&g_cameras[i]);
        if (errval != MI_CAMERA_SUCCESS){
            printf("%s %s: Camera failed to initialize.", __FILE__, __LINE__);
        }
    }

    //errval = mi_SetDeviceChangeCallback((HWND)hwnd, &SimpleCapture::miDevCallBack);

    g_isMidLibInit = 1;
    printf("Midlib initialized.\n");
    return MI_CAMERA_SUCCESS;
}

void setInitPath(char *path)
{

}

int openTransport(ugCamera *cam)
{
    int rval = 0;
 /*   if (!g_isMidLibInit) {
        printf("openTransport() called before initMidLib2() or initMidLib2() previously failed.\n");
        return -1;
    }*/

    mi_camera_t *pCamera = cam->pCamera;

    //pCamera = 0==m_cameraIdx ? gCameras[0] : gCameras[1];
    if (pCamera == NULL) { return -2; }

    if( (rval = pCamera->startTransport(pCamera)) != MI_CAMERA_SUCCESS ) {
        printf("Start Transport Unsuccessful.\n");
        mi_CloseCameras();
        return rval;
    }    

    mi_OpenErrorLog(MI_LOG_SHIP, "ship_log.txt");
    mi_GetErrorLogFileName(errorFileName);
    printf("Log file: %s \n", errorFileName);

    //@@@ iniFilePath, and presetName needs to be passed in from managed code.
	const char* iniFilePath = "C:\\MicrogravityImager.ini";     //path to .ini file.
	const char* presetName = "Demo Initialization Mono";       //settings preset loaded from ini file.
    
	mi_s32 errnum = mi_LoadINIPreset(pCamera, iniFilePath, presetName);
    switch(errnum) {
    case MI_INI_KEY_NOT_SUPPORTED:
        printf("%d: MI_INI_KEY_NOT_SUPPORTED\n", MI_INI_KEY_NOT_SUPPORTED);
        return errnum;
    case MI_INI_LOAD_ERROR:
        printf("%d: MI_INI_LOAD_ERROR\n", MI_INI_LOAD_ERROR);
        return errnum;
    case MI_INI_POLLREG_TIMEOUT:
        printf("%d: MI_INI_POLLREG_TIMEOUT\n", MI_INI_POLLREG_TIMEOUT);
        return errnum;
    }

    //switch logging OFF for some reason...
    pCamera->setMode(pCamera, MI_ERROR_LOG_TYPE, MI_NO_ERROR_LOG);

    //if (cam->nWidth < 1 || cam->nHeight < 1) {
    //    cam->nWidth     = pCamera->sensor->width;
    //    cam->nHeight    = pCamera->sensor->height;
    //}

    cam->nWidth     = pCamera->sensor->width;
    cam->nHeight    = pCamera->sensor->height;
    pCamera->sensor->imageType = MI_BAYER_12;
    pCamera->updateFrameSize(pCamera, pCamera->sensor->width, 
        pCamera->sensor->height, PIXELBITS, 0);
     
    cam->frameSize = pCamera->sensor->width  * 
        pCamera->sensor->height * pCamera->sensor->pixelBytes;

    mallocate(cam);

    mi_u32  fuse1 = 0;
	pCamera->readRegister(pCamera, pCamera->sensor->shipAddr, 0x00FA, &fuse1);

    if (0 != fuse1) {
        cam->camNm = (fuse1==0xb8ce) ? 405 : 485;
    }else{
        printf("Couldn't get Camera Fuse Register for camera: %d\n", cam->camIdx);
        cam->camNm = 0;
    }

    //printCameraInfo(); 

    return 0;
}

unsigned char* doCapture(int cam_idx)
{
    //int cam_idx = g_cameras[0].camNm == cam_nm ? 0 : 1;
    return _doCapture(&g_cameras[cam_idx]);
}

unsigned char *_doCapture(ugCamera *cam)
{
    int nRet;
    int count=0;
    nRet = cam->pCamera->grabFrame(cam->pCamera, 
        cam->pGrabframeBuff, 
        cam->pCamera->sensor->bufferSize); 

    //if (nRet != MI_CAMERA_SUCCESS) {
    //    return NULL;
    //}
    memcpy(cam->pCameraBuff, cam->pGrabframeBuff, cam->frameSize);
    return cam->pCameraBuff;
}


int getWavelength(int camIdx)
{
    return (g_cameras[camIdx]).camNm;
}



void stopTransport(ugCamera *cam)
{
    //close the camera and clean up
    cam->pCamera->stopTransport(cam->pCamera);
    mi_CloseCameras();
    free(cam->pGrabframeBuff);
    free(cam->pCameraBuff);
    mi_CloseErrorLog();
}

int mallocate(ugCamera *cam)
{
     //Allocate a buffer to store the images
    cam->pCameraBuff  = (unsigned char *)malloc(cam->frameSize);
    if (cam->pCameraBuff == NULL) {
        printf("Error trying to create a buffer of size %d to capture %d frames.\n", 
            cam->frameSize, 1);
        mi_CloseCameras();
        return 1;
    }
    //Allocate a buffer to store the images
    cam->pGrabframeBuff  = (unsigned char *)malloc(cam->pCamera->sensor->bufferSize);
    if (cam->pGrabframeBuff==NULL) {
        printf("Error trying to create a buffer of size %d to grab the frames.\n", 
            cam->pCamera->sensor->bufferSize);
        mi_CloseCameras();
        return 1;
    }
    return 0;
}

unsigned long sensorBufferSize(int camIdx)
{
    return g_cameras[camIdx].pCamera->sensor->bufferSize;
}

void printCameraInfo()
{


}



