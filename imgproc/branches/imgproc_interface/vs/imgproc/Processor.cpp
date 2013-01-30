
#include <Windows.h>

#include "Processor.h"
#include "ImageProcessorFactory.h"
#include "ImageProcessor405.h"
#include "ImageProcessor485.h"

#include <string.h>
#include <string>
#include <iostream>
#include <fstream>
#include <utility>

using std::ifstream;
using std::string;
using std::vector;

namespace uG
{
    Processor::Processor(ImageBufferPool *imagePool, DataBufferPool *dataPool) 
        : m_imgproc(NULL)
        , m_imagePool(imagePool)
        , m_dataPool(dataPool)
        , m_stopRequested(false)
    { 
        InitializeCriticalSection(&m_criticalSection);
    }


    Processor::~Processor()
    {
        if (NULL != m_imgproc)    delete m_imgproc;
        DeleteCriticalSection(&m_criticalSection);
    }


    DWORD WINAPI Processor::do_work(LPVOID pArgs)
    {
        typedef vector<string>::iterator vecIt;

        Processor *me = static_cast<Processor*>(pArgs);
        me->m_tid = GetCurrentThreadId();

        while (true) 
        {
            EnterCriticalSection(&(me->m_criticalSection));
                if (me->m_stopRequested) break;
            LeaveCriticalSection(&(me->m_criticalSection));
    
            Buffer<unsigned char> *imgbuf = me->m_imagePool->getFullBuffer();
            Buffer<long long> *longbuf = me->m_dataPool->getFreeBuffer();
            longbuf->id = imgbuf->id; //copy file name.
            AbstractImageProcessor *aip = ImageProcessorFactory::
                getInstance()->newProc(imgbuf->id, imgbuf->data, longbuf->data);
            aip->process();

            me->m_imagePool->returnEmptyBuffer(imgbuf);
            me->m_dataPool->postFullBuffer(longbuf);
            std::cout << me->m_tid << " Processor posted full buffer.\n";
            delete [] aip;
        }
        return 0;
    }

} /* namespace uG */

