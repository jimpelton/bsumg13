

#include "Processor.h"
#include "AbstractImageProcessor.h"
#include "ImageProcessorFactory.h"
#include "Centers.h"

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
    Processor::Processor(ImageBufferPool *imagePool, DataBufferPool *dataPool, uGProcVars *vars ) 
        : m_imagePool(imagePool)
        , m_dataPool(dataPool)
    { 
        InitializeCriticalSection(&m_criticalSection);

        m_tid = 0;
        m_stopRequested = false;
        m_imgproc = ImageProcessorFactory::getInstance()->newProc(vars);
    }


    Processor::~Processor()
    {
        if (NULL != m_imgproc)   delete m_imgproc;
        //DeleteCriticalSection(&m_criticalSection);
    }

    void operator() ()
    {
        while (true) {
            //EnterCriticalSection(&(me->m_criticalSection));
            //    if (me->m_stopRequested) break;
            //LeaveCriticalSection(&(me->m_criticalSection));
    
            Buffer<unsigned char> *imgbuf = me->m_imagePool->getFullBuffer();
            Buffer<long long> *longbuf = me->m_dataPool->getFreeBuffer();
            longbuf->id = imgbuf->id; //copy file name.
            me->m_imgproc->setInput(imgbuf->data);
            me->m_imgproc->setOutput(longbuf->data);
            me->m_imgproc->process();

            me->m_imagePool->returnEmptyBuffer(imgbuf);
            me->m_dataPool->postFullBuffer(longbuf);
            std::cout << me->m_tid << " Processor posted full buffer.\n";
            //delete [] aip;
        }
        return 0;
    }

    //void Processor::do_work(void* pArgs)
    //{

    //    Processor *me = static_cast<Processor*>(pArgs);
    //    me->m_tid = GetCurrentThreadId();

    //    while (true) {
    //        EnterCriticalSection(&(me->m_criticalSection));
    //            if (me->m_stopRequested) break;
    //        LeaveCriticalSection(&(me->m_criticalSection));
    //
    //        Buffer<unsigned char> *imgbuf = me->m_imagePool->getFullBuffer();
    //        Buffer<long long> *longbuf = me->m_dataPool->getFreeBuffer();
    //        longbuf->id = imgbuf->id; //copy file name.
    //        me->m_imgproc->setInput(imgbuf->data);
    //        me->m_imgproc->setOutput(longbuf->data);
    //        me->m_imgproc->process();

    //        me->m_imagePool->returnEmptyBuffer(imgbuf);
    //        me->m_dataPool->postFullBuffer(longbuf);
    //        std::cout << me->m_tid << " Processor posted full buffer.\n";
    //        //delete [] aip;
    //    }
    //    return 0;
    //}

} /* namespace uG */

