
#ifndef _WRITER_H
#define _WRITER_H

#include <Windows.h>

#include "BufferPool.h"
#include "ugTypes.h"
#include "Export.h"

#include <string>

namespace uG
{
    class Writer
    {

    public:
        Writer(const std::string &outPath, DataBufferPool *pool);
        virtual ~Writer();

        static DWORD WINAPI do_work(LPVOID pArgs);

        void writeFile(Buffer<long long> *);

        void requestStop()
        {
            EnterCriticalSection(&m_criticalSection);
            m_stopRequested=true;
            LeaveCriticalSection(&m_criticalSection);
        }

    private:
        std::string m_outPath;
        DataBufferPool *m_pool;
        DWORD m_tid;
        CRITICAL_SECTION m_criticalSection;
        bool m_stopRequested;
    };

} /* namespace uG */


#endif  /* _WRITER_H */
