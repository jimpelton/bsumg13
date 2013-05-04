
#ifndef _WRITER_H
#define _WRITER_H

#ifdef _WIN32
#include <Windows.h>
#endif

#include "BufferPool.h"
#include "ugTypes.h"

#include <boost/thread.hpp>
#include <boost/thread/mutex.hpp>
#include <string>

namespace uG
{
    /**
      *	\brief Writes data to disc.
      *
      *	Changelog:
      *	    [2/22/2013 jim] Port to boost mutex and threads.
      */
    class Writer
    {

    public:
        Writer(const std::string &outPath, DataBufferPool *pool);
        virtual ~Writer();

        void operator()();

        void (Writer::*cb_msg)(void);
        void set_cb(void (Writer::*cb)(void)) {
            cb_msg = cb;
        }

        
        void writeFile(Buffer<long long> *);

        void requestStop()
        {
            m_mutex.lock();
            m_stopRequested=true;
            m_mutex.unlock();
        }

    private:
        std::string m_outPath;
        DataBufferPool *m_pool;
        boost::mutex m_mutex;
        bool m_stopRequested;
    };

} /* namespace uG */


#endif  /* _WRITER_H */
