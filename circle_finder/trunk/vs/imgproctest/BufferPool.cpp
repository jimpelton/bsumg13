#include "BufferPool.h"
#include <Windows.h>

namespace uG
{

    BufferPool::BufferPool(int _nbufs, int _nMaxWait)
        : m_nbufs(_nbufs)
        , m_nMaxWait(_nMaxWait)
        , m_stopRequested(false)
    {
        InitializeCriticalSection(&m_BufferPoolLock);
        InitializeCriticalSection(&m_FullBufferPoolLock);
        InitializeConditionVariable(&m_FreeBuffersAvailable);
        InitializeConditionVariable(&m_FullBuffersAvailable);
        InitializeConditionVariable(&m_FullBufferPoolOpen);

        for (int i = 0; i<_nbufs; i++)
        {
            Buffer *b = new Buffer();
            ZeroMemory(b->data, 96*sizeof(Datum));
            m_BufferPool.push(b);
        }

    }

    BufferPool::~BufferPool()
    {
        DeleteCriticalSection(&m_FullBufferPoolLock);
        DeleteCriticalSection(&m_BufferPoolLock);    
    }

    Buffer* BufferPool::getFreeBuffer()
    {
        EnterCriticalSection(&m_BufferPoolLock);
        while(m_BufferPool.size() == 0)
        {
            SleepConditionVariableCS(&m_FreeBuffersAvailable, &m_BufferPoolLock, INFINITE);
        }

        Buffer *baaahhh = m_BufferPool.top();
        m_BufferPool.pop();
        LeaveCriticalSection(&m_BufferPoolLock);
        return baaahhh;
    }

    void BufferPool::returnEmptyBuffer(Buffer *b)
    {
        EnterCriticalSection(&m_BufferPoolLock);
        m_BufferPool.push(b);
        WakeConditionVariable(&m_FreeBuffersAvailable);
        LeaveCriticalSection(&m_BufferPoolLock);

    }

    void BufferPool::postFullBuffer(Buffer *b)
    {
        EnterCriticalSection(&m_FullBufferPoolLock);
        while (m_FullBufferPool.size() == m_nMaxWait)
        {
            SleepConditionVariableCS(&m_FullBufferPoolOpen, &m_FullBufferPoolLock, INFINITE);
        }

        m_FullBufferPool.push(b);
        WakeConditionVariable(&m_FullBuffersAvailable);
        LeaveCriticalSection(&m_FullBufferPoolLock);

    }

    Buffer* BufferPool::getFullBuffer()
    {
        EnterCriticalSection(&m_FullBufferPoolLock);
        while (m_FullBufferPool.size() == 0)
        {
            SleepConditionVariableCS(&m_FullBuffersAvailable, &m_FullBufferPoolLock, INFINITE);
        }
        Buffer *front = m_FullBufferPool.front();
        m_FullBufferPool.pop();

        WakeConditionVariable(&m_FullBufferPoolOpen);
        LeaveCriticalSection(&m_FullBufferPoolLock);

        return front;
    }


}
