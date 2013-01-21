
#include "Processor.h"
#include "ImageProcessorFactory.h"
#include "ImageProcessor405.h"
#include "ImageProcessor485.h"

#include <Windows.h>
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

    typedef vector<string>::iterator vecIt;

    Processor::Processor(const vector<string> &fileName, BufferPool *pool) 
        : m_imgproc(NULL)
        , m_rawFile(fileName)
        , m_pool(pool)

    { 

        //empty  
    }

    Processor::~Processor()
    {
        if (NULL != m_imgproc)    delete m_imgproc;
    }


    DWORD WINAPI Processor::do_work(LPVOID pArgs)
    {

        Processor *me = static_cast<Processor*>(pArgs);

        me->m_tid = GetCurrentThreadId();
        unsigned char *raw = NULL;
        size_t length;

        vecIt it = me->m_rawFile.begin();
        vecIt itEnd = me->m_rawFile.end();


        for(; it != itEnd; it++)
        {
            if ( me->openImage( *it, &raw, &length ) )
            {
                Buffer *buffer = me->m_pool->getFreeBuffer();
                buffer->id = it->substr(it->find_last_of('\\')+1);
                AbstractImageProcessor *aip = ImageProcessorFactory::
                    getInstance()->getProc(*it, raw, buffer->data);
                aip->process();
                me->m_pool->postFullBuffer(buffer);
                std::cout << me->m_tid << " Processor posted full buffer.\n";
            }
        }
        return 0;
    }

    bool Processor::openImage(const string &fname, 
        unsigned char **rawData, size_t *length)
    {

        ifstream inf(fname.c_str(), std::ios::in | std::ios::binary);

        if (!inf.is_open()) return false;

        inf.seekg(0, std::ios::end);
        size_t offset = inf.tellg();
        inf.seekg(0, std::ios::beg);

        if (*rawData == NULL)
        {
            *rawData = new unsigned char[offset];
            *length=offset;
        }

        ZeroMemory(*rawData, *length);

        inf.read((char*)*rawData, *length);

        inf.close();
        return true;
    }

} /* namespace uG */

