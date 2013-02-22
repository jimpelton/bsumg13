
#ifndef Reader_h__
#define Reader_h__

#ifdef _WIN32
#include <Windows.h>
#endif

#include "BufferPool.h"
#include "Buffer.h"
#include "ugTypes.h"

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

    static DWORD WINAPI do_work(LPVOID aReader);
    //static size_t openSingleImage(const std::string &fname, unsigned char **rawData);
    static size_t openImage(const std::string &fname, unsigned char **rawData);

    void requestStop()
    {
        EnterCriticalSection(&m_criticalSection);
        m_stopRequested=true;
        LeaveCriticalSection(&m_criticalSection);
    }

private:
    Reader(){}

    std::vector<std::string> m_rawFile;
    ImageBufferPool *m_imagePool;

    DWORD m_tid;
    CRITICAL_SECTION m_criticalSection;
    bool m_stopRequested;
    bool m_noThread;



};

} /* namespace uG */
#endif // Reader_h__
