


#ifndef BUFFERPOOL_H
#define BUFFERPOOL_H

#include "Export.h"
#include "Buffer.h"
#include <Windows.h>

#include <queue>
#include <stack>
#include <iostream>

namespace uG
{
/**
  *	\brief Monitor that supports multiple producers and consumers of
  *	any number of recyclable buffers. Full buffers are recycled back to the empty pool.
  *	
  *	On request, hands out an empty buffer to a producer. The consumer may
  *	then post the full buffer back to the BufferPool object upon which
  *	the buffer pool will hand out the full buffer to a waiting consumer.
  *	If there are no waiting consumers the pool will queue the full buffers
  *	until they are requested. An empty buffer is recycled and given to
  *	the next producer that requests an empty buffer.
  *
  * Note that while the BufferPool may easily receive and hand out buffers
  * from many producers and consumers, it can not handle the case where multiple
  * consumers must process the same buffer. That functionality must be handled
  * by something external to the BufferPool.
  * 
  * //  [1/21/2013 jim]
  */
template <  class _Ty > 
class BufferPool
{

    typedef std::queue<Buffer< _Ty > * > BufQueue;
    typedef std::stack<Buffer< _Ty > * > BufPool;

public:
    BufferPool(){}
   /**
     *  \fn BufferPool(size_t nbufs, size_t bufsize, float loadfactor=1.f);
     *  
     *  \param nbufs Number of buffers to generate
     *
     *  \param loadfactor The minimum percentage of empty buffers remaining 
     *  that this bufferpool should reach before allowing more elements to be added. 
     *  If loadfactor=1 (default) then the buffer always allows elements to be added 
     *  if buffer size is less than maxsize.
     *
     *  \param bufsize The max size each buffer is allowed to reach. If maxsize=-1
     *  then the buffers can hold up to the max value of a size_t value
     *  on this architecture.
     */
    BufferPool(size_t nbufs, size_t bufsize, float loadfactor=1.f);
    ~BufferPool();

    /** Checkout empty buffer from pool (LIFO stack) */
    Buffer< _Ty >* getFreeBuffer();
    /** Checkout full buffer from pool (FIFO queue) */
    Buffer< _Ty >* getFullBuffer();

    /** Commit a full buffer to the pool FIFO queue */
    void postFullBuffer(Buffer< _Ty > *);
    /** Commit an empty buffer to the pool LIFO stack */
    void returnEmptyBuffer(Buffer< _Ty > *);

private:
    size_t m_nbufs;         /// Maximum number of buffers
    size_t m_nMinFillCount; /// Number of buffs to start filling at.
    size_t m_bufsize;       /// Number of elements in each buffer.
    bool m_stopRequested;   /// True if the buffer should stop before the next exec.

    CRITICAL_SECTION m_FullBufferPoolLock;
    CRITICAL_SECTION m_BufferPoolLock;

    CONDITION_VARIABLE m_FreeBuffersAvailable;
    CONDITION_VARIABLE m_FullBuffersAvailable;
    CONDITION_VARIABLE m_FullBufferPoolOpen;

    BufQueue m_FullBufferPool; /// full buffs.
    BufPool m_BufferPool;      /// empty buffs.
};
/************************************************************************/
/*    BufferPool Implementation                                        */
/************************************************************************/
template < class _Ty >
BufferPool< _Ty >::BufferPool(size_t nbufs, size_t bufsize, float loadfactor)
    : m_nbufs(nbufs)
    , m_bufsize(bufsize)
    , m_stopRequested(false)
{
    InitializeCriticalSection(&m_BufferPoolLock);
    InitializeCriticalSection(&m_FullBufferPoolLock);
    InitializeConditionVariable(&m_FreeBuffersAvailable);
    InitializeConditionVariable(&m_FullBuffersAvailable);
    InitializeConditionVariable(&m_FullBufferPoolOpen);
    
    if (loadfactor < 0.f || loadfactor > 1.f) { loadfactor=1.f; }
    m_nMinFillCount = loadfactor * m_nbufs;

    for (size_t i = 0; i<nbufs; i++)
    {
        Buffer< _Ty > *b = new Buffer< _Ty >(bufsize);
        m_BufferPool.push(b);
    }

};

template < class _Ty >
BufferPool< _Ty >::~BufferPool()
{
    DeleteCriticalSection(&m_FullBufferPoolLock);
    DeleteCriticalSection(&m_BufferPoolLock);    
    //TODO: clean up buffers arrays.
};

template < class _Ty >
Buffer< _Ty >* 
BufferPool< _Ty >::getFreeBuffer()
{
    EnterCriticalSection(&m_BufferPoolLock);
    while(m_BufferPool.size() == 0)
    {
        SleepConditionVariableCS(&m_FreeBuffersAvailable, &m_BufferPoolLock, INFINITE);
    }

    Buffer< _Ty > *baaahhh = m_BufferPool.top();
    m_BufferPool.pop();
    DWORD tid = GetCurrentThreadId();
    std::cout << tid << ": Checking out free buffer." << std::endl;
    LeaveCriticalSection(&m_BufferPoolLock);
    return baaahhh;
};

template < class _Ty >
void 
BufferPool< _Ty >::returnEmptyBuffer(Buffer< _Ty > *b)
{
    EnterCriticalSection(&m_BufferPoolLock);
    m_BufferPool.push(b);
    WakeConditionVariable(&m_FreeBuffersAvailable);
    DWORD tid = GetCurrentThreadId();
    std::cout << tid << ": Checking in empty buffer." << std::endl;
    LeaveCriticalSection(&m_BufferPoolLock);
};

template < class _Ty >
void 
BufferPool< _Ty >::postFullBuffer(Buffer< _Ty > *b)
{
    EnterCriticalSection(&m_FullBufferPoolLock);
    while (m_FullBufferPool.size() == m_nbufs ||
           m_FullBufferPool.size() >  m_nMinFillCount) 
    {
        SleepConditionVariableCS(&m_FullBufferPoolOpen, &m_FullBufferPoolLock, INFINITE);
    }

    m_FullBufferPool.push(b);
    WakeConditionVariable(&m_FullBuffersAvailable);
    DWORD tid = GetCurrentThreadId();
    std::cout << tid << ": Checking in full buffer." << std::endl;
    LeaveCriticalSection(&m_FullBufferPoolLock);

};

template < class _Ty >
Buffer< _Ty >* 
BufferPool< _Ty >::getFullBuffer()
{
    EnterCriticalSection(&m_FullBufferPoolLock);
    while (m_FullBufferPool.size() == 0)
    {
        SleepConditionVariableCS(&m_FullBuffersAvailable, &m_FullBufferPoolLock, INFINITE);
    }
    Buffer< _Ty > *front = m_FullBufferPool.front();
    m_FullBufferPool.pop();
    DWORD tid = GetCurrentThreadId();
    std::cout << tid << ": Checking out full buffer." << std::endl;
    WakeConditionVariable(&m_FullBufferPoolOpen);
    LeaveCriticalSection(&m_FullBufferPoolLock);

    return front;
};
} /* namespace uG */

#endif // BUFFERPOOL_H
