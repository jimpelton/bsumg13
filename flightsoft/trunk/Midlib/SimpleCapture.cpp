#include <Windows.h>

#include "SimpleCapture.h"
#include <sstream>
#include <string>


mi_camera_t *gCameras[MAX_CAMS]; 
bool gDone = FALSE;						// Set to cause all workers to exit
HANDLE gBarrierSemaphore;				// Synchronization object for all threads
volatile unsigned int gWaitingCount;	// Number of waiters, start next loop when this reaches 4

int SimpleCapture::num_cameras=0;
bool SimpleCapture::isMidLibInit=false;


DWORD WINAPI 
SimpleCapture::captureFunction(LPVOID param)
{
	DWORD rv;
    SimpleCapture *me = (SimpleCapture*)param;
	unsigned int currValue;

	while (1) {
		currValue = InterlockedIncrement(&gWaitingCount);		// Atomic increment
		if (currValue == NUM_WORKER_THREADS) {
			// I'm the last thread here so I don't need to sleep, just wake up others and go
			gWaitingCount = 0;
			// Releasing the semaphore NUM_WORKER_THREADS times unblocks that many threads
			ReleaseSemaphore(gBarrierSemaphore, NUM_WORKER_THREADS - 1, 0);	// Minus 1 because this thread doesn't sleep
		} else {
			rv = WaitForSingleObject(gBarrierSemaphore, INFINITE);	// Other threads wait for main thread to start
			if (rv != WAIT_OBJECT_0) {
				printf("Wait failed (%d)\n", GetLastError());
				return 1;
			}
		}

        me->_doCapture();

		// if gDone was set by the main thread, then exit
		if (gDone) {
			return 0;
		}
		

	} /* while(1) */

    return 0;
}

SimpleCapture::SimpleCapture()
    : m_cameraIdx(-1)
    , m_nextFrameIdx(0) 
    , num_frames(1)
    , nWidth(0)
    , nHeight(0)
    , m_camNM(0) { }

SimpleCapture::~SimpleCapture(void) { }

//return: 0 success, > 0 on error
int 
SimpleCapture::initMidLib2(int nCamsReq)
{
    if (isMidLibInit) return 0;

	//TODO: possible memory leak here?
	gBarrierSemaphore = CreateSemaphore(NULL, 0, 1024, TEXT("Barrier Semaphore"));
	gWaitingCount=0;
    
    if (nCamsReq <= 0) {
        printf("You requested %d cams!" \
            "That is not a good amount of cams to request!\n", 
            nCamsReq);
        return 1;
    }

    mi_s32 errval = mi_OpenCameras(gCameras, 
		&SimpleCapture::num_cameras, mi_SensorData());
    
    if (num_cameras < nCamsReq) {
        printf("Could not initialize %d camera(s). Found %d. \n",
            nCamsReq, num_cameras);
        mi_CloseCameras();

        return errval;
    } 

    isMidLibInit=true;

    printf("Midlib initialized.\n");
    return 0;
}

//return: 0 success, > 0 on failure.
int 
SimpleCapture::openTransport(int camIdx)
{
	int rval = 0;
    if (!isMidLibInit) {
        printf("openTransport() called before initMidLib2().\n");
        return 1;
    }

    m_cameraIdx=camIdx;
    pCamera = 0==m_cameraIdx ? gCameras[0] : gCameras[1];

    if( (rval = pCamera->startTransport(pCamera)) != MI_CAMERA_SUCCESS ) {
        printf("Start Transport Unsuccessful.\n");
        mi_CloseCameras();
        return rval;
    }    

    mi_OpenErrorLog(MI_LOG_SHIP, "ship_log.txt");
    mi_GetErrorLogFileName(errorFileName);
    printf("Log file: %s \n", errorFileName);

    std::stringstream iniFilePath;
    char lpCwdBuf[100];
    GetCurrentDirectory(100, lpCwdBuf);
	iniFilePath << lpCwdBuf << "\\MicrogravityImager.ini";
	std::string iniDir = iniFilePath.str();
	const char* presetNamePtr = "Demo Initialization Mono";
    
    mi_s32 errnum = mi_LoadINIPreset(pCamera, iniDir.c_str(), presetNamePtr);
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

    pCamera->sensor->imageType = MI_PNG;
    pCamera->updateFrameSize(pCamera, nWidth, nHeight, PIXELBITS,0);
     
    frameSize = pCamera->sensor->width * pCamera->sensor->height
              * pCamera->sensor->pixelBytes;

    mallocate();

    mi_u32  fuse1 = 0;
	pCamera->readRegister(pCamera,pCamera->sensor->shipAddr, 0x00FA, &fuse1);

    if (0 != fuse1) {
        m_camNM = (fuse1==0xb8ce) ? 405 : 485;
    }else{
        printf("couldn't get Camera Fuse Register for camera: %d\n", m_cameraIdx);
        m_camNM=m_cameraIdx;
    }

    //printShit(); //unprofessional, but accurate. 

    return 0;
}


unsigned char * 
SimpleCapture::_doCapture()
{
    char fname[30];
    //sprintf(fname, "Camera%d_%d.raw", m_camNM, m_nextFrameIdx++);
    //imfile = fopen(fname,"wb");
    //TODO: error checking for open file!
    // 
    //grabFrame 
    //printf("Grabbing frame: %s\n", fname);
    for (frame = 0; frame < num_frames; frame++) {
        int    nRet;
        //mi_u8  tempByte;
        //int    i;

        /* Skip frames until a good frame is found.  */
        int count=0;
        nRet = pCamera->grabFrame(pCamera, pGrabframeBuff, pCamera->sensor->bufferSize);
        if (nRet != MI_CAMERA_SUCCESS) {
            return NULL;
        }
        memcpy(pCameraBuff, pGrabframeBuff, frameSize);
        //fwrite(pCameraBuff, frameSize, 1, imfile);
    }
    printf("[done]\n");
    //fclose(imfile); 
    return pCameraBuff;
}

void
SimpleCapture::stopTransport()
{
    //stop the camera and clean up
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
    if (pCameraBuff==NULL) {
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

int SimpleCapture::getWavelength()
{
	return m_camNM;
}

void 
SimpleCapture::printShit()
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

