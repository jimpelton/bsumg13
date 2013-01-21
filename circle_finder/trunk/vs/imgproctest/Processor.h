
#ifndef _PROCESSOR_H
#define _PROCESSOR_H

#include "AbstractImageProcessor.h"
#include "BufferPool.h"

#include <Windows.h>
#include <string>
#include <vector>


namespace uG
{
    /*!
    *  \class Processor
    *  
    *  
    */

    class Processor
    {

    public:
        Processor(const std::vector<std::string> &fileName, BufferPool *);
        virtual ~Processor();

        static DWORD WINAPI do_work(LPVOID pArgs);


    private:
        bool openImage(const std::string &fname, unsigned char **rawData, size_t *length);

        AbstractImageProcessor *m_imgproc;
        std::vector<std::string> m_rawFile;
        BufferPool *m_pool;
        DWORD m_tid;
        CRITICAL_SECTION m_criticalSection;
    };

} /* namespace uG */











#endif /* _PROCESSOR_H */
