#include <Windows.h>

#include "SimpleCapture.h"
#include <string>
#include <iostream>


extern bool gDone ; //= FALSE;						// Set to cause all workers to exit
extern HANDLE gBarrierSemaphore;				// Synchronization object for all threads
extern volatile unsigned int gWaitingCount;	// Number of waiters, start next loop when this reaches 4

SimpleCapture sc[2];

int main(int argc, char *argv[])
{ 
    int numFramesRequested=1, 
        camsRequested=1,
        nHeight=1,
        nWidth=1; 


    //parse command line for the number of frames to capture and 
    //the width and height (if not default)
    switch (argc)
    {
    case 4: 
        nWidth     = atoi(argv[2]);
        nHeight    = atoi(argv[3]);
    case 2: 
        numFramesRequested = atoi(argv[1]);
        break;
    case 1:
        break;
    default:    
        printf("usage:\n");
        printf("    SimpleCapture\n");
        printf("    SimpleCapture <num_frames>\n");
        printf("    SimpleCapture <num_frames> <width> <height>\n");
        return 0;
    }
    if (1 == SimpleCapture::initMidLib2(camsRequested))
    {
        printf("Aieeee! Couldn't initialize midlib2!!\n");
        return 0;
    }
    
    if (0 != sc[0].openTransport(0))
    {
        printf("openTransport failed on Camera 0. Exiting...\n");
        return 0;
    }
    if (0 != sc[1].openTransport(1))
    {
        printf("openTransport() failed on Camera 1. Exiting...\n");
        return 0;
    }

    HANDLE lpThreads[2];
    LPDWORD pdwThreadIds[2]; 
    pdwThreadIds[0] = NULL;
    pdwThreadIds[1] = NULL;

	for (int i=0; i<2; ++i)
	{
        lpThreads[i] = CreateThread(
            NULL, 
            0, 
            SimpleCapture::captureFunction, 
            (void*)&sc[i],
            CREATE_SUSPENDED,
            pdwThreadIds[i]
        );

        if (!lpThreads[i])
        {
            std::cerr << "Unable to create thread " << i << std::endl;
            ExitProcess(1);
        }
	}

    ResumeThread(lpThreads[0]);
    ResumeThread(lpThreads[1]);

    char c;
	while(!gDone) {
		c = _getch();
		if (c == 'q') {
			gDone = true;
			break;
		}
	}

    ReleaseSemaphore(gBarrierSemaphore, NUM_WORKER_THREADS, 0);
	WaitForMultipleObjects(NUM_WORKER_THREADS, lpThreads, TRUE, INFINITE);
	CloseHandle(gBarrierSemaphore);

    return 0; 
}