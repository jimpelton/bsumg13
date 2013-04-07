
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
#include <boost/thread/mutex.hpp>

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
            m_mutex.lock();
                m_stopRequested=true;
            m_mutex.unlock();
        }

    private:
        AbstractImageProcessor *m_imgproc;  

        /// Pool of image (input) buffers. 
        ImageBufferPool *m_imagePool;

        /// Pool of data (output) buffers. 
        DataBufferPool *m_dataPool;

        boost::mutex m_mutex;
        bool m_stopRequested;
    };
} /* namespace uG */











#endif /* _PROCESSOR_H */
