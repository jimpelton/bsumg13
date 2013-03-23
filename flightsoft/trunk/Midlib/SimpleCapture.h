#ifndef SimpleCapture_h__
#define SimpleCapture_h__

#include <stdio.h>
#include <stdlib.h>
#include <string>
#include <string.h>
#include <conio.h>

#include <midlib2.h>

#define NUM_WORKER_THREADS      2 
#define DEFAULT_NUM_FRAMES      1
#define PIXELBITS               16
#define MAX_BADFRAME_TRIES      10

#define MAX_CAMS 2

/*
 *	Captures images. 
 *	Pass captureFunction to a win32 thread for greatest happiness.
 *	
 *	Modified from the SimpleCapture example provided by Aptina.
 */
class __declspec(dllexport)  SimpleCapture
{
public:

  /************************************************************************/
  /* PUBLIC STATIC MEMBERS                                                */
  /************************************************************************/ 
    static mi_s32 num_cameras;
    static bool isMidLibInit;

    /** 
     * \brief Init all cams, make sure at least nCamsReq 
     * cameras were initialized.
     * \return 1 on failure, 0 otherwise.
     */
    static int initMidLib2(int nCamsReq);

    /**
     * \brief Pass to worker thread for performing the capture.
     */
    static DWORD WINAPI captureFunction(LPVOID args);

  /************************************************************************/
  /* PUBLIC MEMBERS                                                       */
  /************************************************************************/ 
    //mi_s32          num_cameras;                           //number of cameras found
    //mi_camera_t     *cameras[MI_MAX_CAMERAS];              //cameras found
    mi_camera_t     *pCamera; //=NULL;                     //current camera
    unsigned long   frameSize;                             //size of the frames we want to save
    unsigned char   *pCameraBuff;                          //memory buffer for all the sensor images
    unsigned char   *pGrabframeBuff;                       //grabFrame buffer
    unsigned int    num_frames;// = DEFAULT_NUM_FRAMES;    //number of frames to capture
    unsigned long   nWidth;//  = 0;                        //width of image (taken from command line or set to default below)
    unsigned long   nHeight;// = 0;                        //height of image (taken from command line or set to default below)
    FILE            *imfile;                               //capture file
    unsigned int    frame;
    mi_u32          nSwizzleNeeded;
    mi_string       errorFileName;
    char            imagetypestr[32];

  /************************************************************************/
  /* PUBLIC METHODS                                                       */
  /************************************************************************/ 

    SimpleCapture();
    ~SimpleCapture();

	int getWavelength();

    /**
     *	\brief Open the midlib2 transport for the given camera index.
     *	
     *	initMidLib2() must be called prior to calling openTransport(). The
     *	value passed into camidx sets the index value for this camera.
	 *  The return value is the MI_CAMERA_* error code.
     *	
     *	\param camidx The camera index to open transport for. Must be <= to the number
     *	of cameras that midlib2 found.
     *
     *  \return 0 on success, > 0 on error.
     *  
     */
    int  openTransport(int camidx); 

    /** 
     *  \brief Stop the transport for this SimpleCapture's camera. 
     */
    void stopTransport();

    unsigned long sensorBufferSize();
    unsigned char* _doCapture();

  /************************************************************************/
  /* PRIVATE members                                                      */
  /************************************************************************/ 
private:

    int m_cameraIdx;
    int m_nextFrameIdx;
    int m_camNM;
    
    /** \brief Called from captureFunction to perform the capture. */

    int mallocate();
    void printShit();
};

public ref class ManagedSimpleCapture{
    SimpleCapture *native_sc;

public:
    ManagedSimpleCapture() 
        : native_sc(new SimpleCapture()) {}
    ~ManagedSimpleCapture() { delete native_sc; }

    /**
     *	\brief CLR Wrapper function around openTransport
	 * 
     *	\param camidx The camera index to open transport for. Must be <= to the number
     *	of cameras that midlib2 found.
	 * 
     *  \return 1 on error, 0 on success.
     */
    int managed_OpenTransport(int camidx)
    {
        return native_sc->openTransport(camidx);
    }

	  /**
	  *	\brief Get the wave
	  */
	int managed_GetWavelength()
	{
		return native_sc->getWavelength();
	}



	/**
	  *	\brief CLR wrapper around sensorBufferSize().
	  */
    unsigned long managed_SensorBufferSize()
    {
        return native_sc->sensorBufferSize();
    }

	/**
	  *	\brief CLR wrapper around stopTransport.
	  */
    void managed_StopTransport()
    {
        native_sc->stopTransport();
    }

	/**-
	  *  \brief CLR wrapper around _doCapture.
	  *  Note: _doCapture writes the captured file to disk, so
	  *  if you are planning on doing that yourself
	  */
    unsigned char* managed_DoCapture()
    {
        return native_sc->_doCapture();
    }

    static int managed_InitMidLib(int nCamsReqd)
    {
        return SimpleCapture::initMidLib2(nCamsReqd);
    }


};

static void 
str_replace(char* strOut, char* strIn, char* oldStr, char* newStr)
{
    int total_len = (int)strlen(strIn);
    char* pChar;
    int lenBeg;

    pChar= strstr(strIn,oldStr);
    lenBeg = total_len-(int)strlen(pChar);
    strncpy(strOut, strIn,lenBeg);
    strOut[lenBeg] = 0;
    strcat(strOut,newStr);
    strcat(strOut,&strIn[lenBeg+strlen(oldStr)]);
}

#endif // SimpleCapture_h__
