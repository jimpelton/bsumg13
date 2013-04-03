
#ifndef Reader_h__
#define Reader_h__

#ifdef _WIN32
#include <Windows.h>
#endif

#include "BufferPool.h"
#include "Buffer.h"
#include "ugTypes.h"

#include <boost/thread.hpp>
#include <boost/thread/mutex.hpp>

#include <string>
#include <vector>


namespace uG {
/**
  *	\brief Reads any file from disc and places the data into a buffer pool of char.
 */
class Reader
{
public:
    Reader(const std::vector<std::string> &fileNames, ImageBufferPool *imagePool);
    ~Reader(void);
    
    void operator()(); 

    static size_t openImage(const std::string &fname, unsigned char **rawData);

    void requestStop()
    {
        m_mutex.lock();
        m_stopRequested=true;
        m_mutex.unlock();
    }

private:
    Reader(){}

    std::vector<std::string> m_rawFile;
    ImageBufferPool *m_imagePool;

    bool m_stopRequested;
    //bool m_noThread;
    boost::mutex m_mutex;
};

} /* namespace uG */
#endif // Reader_h__
