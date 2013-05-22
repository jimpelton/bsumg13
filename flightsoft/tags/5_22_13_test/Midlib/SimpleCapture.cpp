#include <Windows.h>

#include "SimpleCapture.h"
#include <sstream>
#include <string>

using std::string;

mi_camera_t *gCameras[MAX_CAMS]; 

int SimpleCapture::num_cameras=0;
bool SimpleCapture::isMidLibInit=false;

SimpleCapture::SimpleCapture()
    : m_cameraIdx(-1)
    , m_nextFrameIdx(0) 
    , num_frames(1)
    , nWidth(0)
    , nHeight(0)
    , m_camNM(0) 
{

    iniFilePath = new char[260];
}

SimpleCapture::~SimpleCapture(void) 
{
    delete iniFilePath;
}

mi_u32 
SimpleCapture::miDevCallBack(HWND hwnd, _mi_camera_t *pCamera, mi_u32 flags)
{
    MessageBox(hwnd, TEXT("holy jesus fucking christ."), TEXT("eeeee"), MB_OK);
    if (flags & MI_DEVEVENT_REMOVAL)
    {
        printf("Aieeeeeeeeeeeeeeeeeeeeeeeeeeeeee!\n");
    }
    return MI_CAMERA_SUCCESS;
}

//return: 0 success, > 0 on error
int 
SimpleCapture::initMidLib2(int nCamsReq)
{
    if (isMidLibInit) return 0;

	//TODO: this line leaks memory if initMidLib2 is called multiple times
	//gBarrierSemaphore = CreateSemaphore(NULL, 0, 1024, TEXT("Barrier Semaphore"));
	//gWaitingCount=0;
    
    if (nCamsReq <= 0) {
        printf("You requested %d cams!" \
            "That is not a good number of cams to request!\n", 
            nCamsReq);
        return 1;
    }

    mi_s32 errval = mi_OpenCameras(gCameras, 
                    &SimpleCapture::num_cameras, mi_SensorData());

    if (errval != MI_CAMERA_SUCCESS) {
        return errval;
    }

    //errval = mi_SetDeviceChangeCallback((HWND)hwnd, &SimpleCapture::miDevCallBack);

    isMidLibInit=true;
    printf("Midlib initialized.\n");

 /*   if (num_cameras < nCamsReq) {
        printf("Could not initialize %d camera(s). Found %d. \n",
            nCamsReq, num_cameras);
        mi_CloseCameras();

        return errval;
    } */

    return 0;
}

//return: 0 success, > 0 on failure.
// -1 midlib not initialized
// -2 likely no cameras were found.
int 
SimpleCapture::openTransport(int camIdx)
{
	int rval = 0;
    if (!isMidLibInit) {
        printf("openTransport() called before initMidLib2() or initMidLib2() previously failed.\n");
        return -1;
    }

    m_cameraIdx=camIdx;
    pCamera = 0==m_cameraIdx ? gCameras[0] : gCameras[1];
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

    if (nWidth < 1 || nHeight < 1) {
        nWidth     = pCamera->sensor->width;
        nHeight    = pCamera->sensor->height;
    }

    
    pCamera->sensor->imageType = MI_BAYER_12;
    pCamera->updateFrameSize(pCamera, nWidth, nHeight, PIXELBITS, 0);
     
    frameSize = pCamera->sensor->width * pCamera->sensor->height
              * pCamera->sensor->pixelBytes;

    mallocate();

    mi_u32  fuse1 = 0;
	pCamera->readRegister(pCamera, pCamera->sensor->shipAddr, 0x00FA, &fuse1);

    if (0 != fuse1) {
        m_camNM = (fuse1==0xb8ce) ? 405 : 485;
    }else{
        printf("couldn't get Camera Fuse Register for camera: %d\n", m_cameraIdx);
        m_camNM=m_cameraIdx;
    }

    //printCameraInfo(); 

    return 0;
}


unsigned char * 
SimpleCapture::_doCapture()
{
    //TODO: remove this loop because num_frames should always be 1.
    for (frame = 0; frame < num_frames; frame++) {
        int    nRet;
        int count=0;
        nRet = pCamera->grabFrame(pCamera, pGrabframeBuff, pCamera->sensor->bufferSize); 
        if (nRet != MI_CAMERA_SUCCESS) {
            return NULL;
        }
        memcpy(pCameraBuff, pGrabframeBuff, frameSize);
    }
    return pCameraBuff;
}

void
SimpleCapture::stopTransport()
{
    //close the camera and clean up
    pCamera->stopTransport(pCamera);
    mi_CloseCameras();
    free(pGrabframeBuff);
    free(pCameraBuff);
    mi_CloseErrorLog();
}

unsigned long
SimpleCapture::sensorBufferSize()
{
    return pCamera->sensor->bufferSize;
}

int 
SimpleCapture::mallocate()
{
    //Allocate a buffer to store the images
    pCameraBuff  = (unsigned char *)malloc(frameSize);
    if (pCameraBuff == NULL) {
        printf("Error trying to create a buffer of size %d to capture %d frames.\n", 
            frameSize, num_frames);
        mi_CloseCameras();
        return 1;
    }
    //Allocate a buffer to store the images
    pGrabframeBuff  = (unsigned char *)malloc(pCamera->sensor->bufferSize);
    if (pGrabframeBuff==NULL) {
        printf("Error trying to create a buffer of size %d to grab the frames.\n", 
            pCamera->sensor->bufferSize);
        mi_CloseCameras();
        return 1;
    }
    return 0;
}

int 
SimpleCapture::getWavelength()
{
	return m_camNM;
}

void 
SimpleCapture::printCameraInfo()
{
    printf("======================\n");
    printf("Camera: %d Wavelength: %d info\n", m_cameraIdx, m_camNM);
    mi_GetImageTypeStr(pCamera->sensor->imageType, imagetypestr);
    printf("sensorName = %s \n", pCamera->sensor->sensorName);
    printf("sensorType = %d \n", pCamera->sensor->sensorType);
    printf("width = %d \n", pCamera->sensor->width);
    printf("height = %d \n", pCamera->sensor->height);
    printf("pixelBytes = %d \n", pCamera->sensor->pixelBytes);
    printf("pixelBits = %d \n", pCamera->sensor->pixelBits);
    printf("bufferSize = %d \n", pCamera->sensor->bufferSize);
    printf("imageType = %s \n", imagetypestr);
    printf("shipAddr = %d\n", pCamera->sensor->shipAddr);
    printf("num_regs = %d \n", pCamera->sensor->num_regs);
}

