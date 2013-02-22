
#ifndef _WRITER_H
#define _WRITER_H

#ifdef _WIN32
#include <Windows.h>
#endif

#include "BufferPool.h"
#include "ugTypes.h"
#include "Export.h"

#include <string>

namespace uG
{
    /**
      *	\brief Writes data to disc.
      */
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
