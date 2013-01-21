#ifndef BUFFERPOOL_H
#define BUFFERPOOL_H

#include <Windows.h>

#include <queue>
#include <stack>

namespace uG
{



typedef long long Datum;
typedef Datum* BufferArray;

struct Buffer 
{
    Datum data[96];
    std::string id;

    Buffer(const std::string &_id = std::string()) : id(_id) { }
};


typedef std::queue<Buffer*> BufQueue;
typedef std::stack<Buffer*> BufPool;

class BufferPool
{
public:
    BufferPool(int _nbufs, int _nMaxWait);
    ~BufferPool();

    Buffer* getFreeBuffer();
    Buffer* getFullBuffer();
    void postFullBuffer(Buffer*);
    void returnEmptyBuffer(Buffer*);

private:
    int m_nbufs;
    int m_nMaxWait;
    bool m_stopRequested;

    CRITICAL_SECTION m_FullBufferPoolLock;
    CRITICAL_SECTION m_BufferPoolLock;

    CONDITION_VARIABLE m_FreeBuffersAvailable;
    CONDITION_VARIABLE m_FullBuffersAvailable;
    CONDITION_VARIABLE m_FullBufferPoolOpen;

    BufQueue m_FullBufferPool;
    BufPool m_BufferPool;
};
} /* namespace uG */
#endif // BUFFERPOOL_H
