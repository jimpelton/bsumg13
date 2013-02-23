
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

        void requestStop()
        {
            lock(m_mutex);
                m_stopRequested=true;
            unlock(m_mutex);
        }

    private:
        AbstractImageProcessor *m_imgproc;  

        /// Pool of image (input) buffers. 
        ImageBufferPool *m_imagePool;

        /// Pool of data (output) buffers. 
        DataBufferPool *m_dataPool;

        /// This thread id.
        boost::thread::id m_tid;
        boost::mutex m_mutex;
        bool m_stopRequested;
    };
} /* namespace uG */











#endif /* _PROCESSOR_H */
