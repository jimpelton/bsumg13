

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
        m_stopRequested = false;
        m_imgproc = ImageProcessorFactory::getInstance()->newProc(vars);
    }


    Processor::~Processor()
    {
        if (m_imgproc != NULL)   delete m_imgproc;
    }

    void Processor::operator() ()
    {
        while (true) {
    
            m_mutex.lock();
            if (m_stopRequested) {
                m_mutex.unlock();
                break;
            }
            m_mutex.unlock();

            Buffer<unsigned char> *imgbuf = m_imagePool->getFullBuffer();
            Buffer<long long> *longbuf = m_dataPool->getFreeBuffer();
            longbuf->id = imgbuf->id; //copy file name.
            m_imgproc->setInput(imgbuf->data);
            m_imgproc->setOutput(longbuf->data);
            m_imgproc->process();

            m_imagePool->returnEmptyBuffer(imgbuf);
            m_dataPool->postFullBuffer(longbuf);
        }
    }


} /* namespace uG */

