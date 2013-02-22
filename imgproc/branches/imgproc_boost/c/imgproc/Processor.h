
#ifndef _PROCESSOR_H
#define _PROCESSOR_H

#ifdef _WIN32
#include <Windows.h>
#endif


#include "AbstractImageProcessor.h"
#include "BufferPool.h"
#include "Export.h"
#include "ugTypes.h"
#include "Centers.h"

#include <string>
#include <vector>
#include <boost/thread.hpp>

namespace uG
{
    /**
    *  \brief Processes images with an AbstractImageProcessor.
    */
    class Processor
    {
    public:
        Processor(ImageBufferPool *imagePool, DataBufferPool *dataPool, uGProcVars *vars);
        virtual ~Processor();

        void operator()();

        //static void do_work(void *aProcessor);

        void requestStop()
        {
            EnterCriticalSection(&m_criticalSection);
                m_stopRequested=true;
            LeaveCriticalSection(&m_criticalSection);
        }

    private:
        AbstractImageProcessor *m_imgproc;  

        /// Pool of image (read) buffers. 
        ImageBufferPool *m_imagePool;

        /// Pool of data (write) buffers. 
        DataBufferPool *m_dataPool;

        /// This thread id.
        boost::thread::id m_tid;
        //CRITICAL_SECTION m_criticalSection;
        bool m_stopRequested;
    };
} /* namespace uG */











#endif /* _PROCESSOR_H */
