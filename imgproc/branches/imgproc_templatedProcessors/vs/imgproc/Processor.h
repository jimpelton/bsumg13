
#ifndef _PROCESSOR_H
#define _PROCESSOR_H

#include "AbstractImageProcessor.h"
#include "BufferPool.h"
#include "Export.h"
#include "ugTypes.h"
#include "Centers.h"

#include <Windows.h>
#include <string>
#include <vector>


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

        static DWORD WINAPI do_work(LPVOID aProcessor);

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
        DWORD m_tid;
        CRITICAL_SECTION m_criticalSection;
        bool m_stopRequested;
    };
} /* namespace uG */











#endif /* _PROCESSOR_H */
