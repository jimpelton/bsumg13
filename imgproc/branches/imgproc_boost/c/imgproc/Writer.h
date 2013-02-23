
#ifndef _WRITER_H
#define _WRITER_H

#ifdef _WIN32
#include <Windows.h>
#endif

#include "BufferPool.h"
#include "ugTypes.h"
#include "Export.h"
#include <boost/thread.hpp>
#include <boost/thread/mutex.hpp>
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

        void operator()();

        void writeFile(Buffer<long long> *);

        void requestStop()
        {

            m_stopRequested=true;

        }

    private:
        std::string m_outPath;
        DataBufferPool *m_pool;
        boost::mutex m_mutex;
        bool m_stopRequested;
    };

} /* namespace uG */


#endif  /* _WRITER_H */
