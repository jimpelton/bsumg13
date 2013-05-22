#include <Windows.h>
#include <process.h>
#include <fileapi.h>

#include <iostream>
#include <fstream>
#include <sstream>

#include "SimpleCapture2.h"

struct camidx{
    int index;
    char * dir;
};

unsigned __stdcall thread_func(void *arg)
{
    camidx cidx = *(camidx*)arg;
    
    int count = 0;
    
    while (true)
    {
        unsigned char * buf = doCaptureIdx(cidx.index);
        std::stringstream ss;
        ss << cidx.dir << "Cam" << cidx.index << "_" << count << ".raw";
        
        std::cout << "saving: " << ss.str() << std::endl;

        std::fstream f(ss.str().c_str(), std::ios::out | std::ios::binary);
        if (!f.is_open()) {
            std::cout << "Couldn't open file." << std::endl;
        }

        f.write((char*)buf, 2592*1944*2);
        count++;
    }
}

int main(int argc, char* argv[])
{
    char* dir = "C:\\TestDir\\";
    LPCTSTR lpsDir = TEXT("C:\\TestDir\\");   //omg! windows is dumb.

    BOOL create_err = CreateDirectory(lpsDir, NULL);
    if (!create_err)
    {
        std::cout << "Couldn't create directory." << std::endl;
        ExitProcess(0);
    }

    int err = initMidLib2(2);
    if (err)
    {
        std::cout << "Midlib wasn't initialized." << std::endl;
        ExitProcess(0);
    }

    camidx camIndexes[2];
    int numThreads = 2;
    LPHANDLE phThreads = new HANDLE[numThreads];
    unsigned int *pdwThreadIds = new unsigned int[numThreads];

    for (int i=0; i<numThreads; ++i)
    {
        camIndexes[i].index = i;
        camIndexes[i].dir = (char*)dir;

        phThreads[i] = (HANDLE) _beginthreadex(
            NULL, 
            0,
            thread_func,
            (void*)&camIndexes[i],
            0,
            &pdwThreadIds[i]
        );

        if (!phThreads[i])
        {
            std::cerr << "Commencing freakout. Couldn't initialize threads: " << i << std::endl;
            ExitProcess(1);
        }
    }

    WaitForMultipleObjects(numThreads, phThreads, TRUE, INFINITE);

    delete [] phThreads;
    delete [] pdwThreadIds;

    ExitProcess(0);

}

