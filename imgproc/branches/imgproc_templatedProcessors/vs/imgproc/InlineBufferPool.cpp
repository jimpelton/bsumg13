//#include <Windows.h>
//
//#include "InlineBufferPool.h"
//
//namespace uG
//{
//
//    InlineBufferPool::InlineBufferPool(int nbufs, float loadFactor, bool initBuffs = false)
//        : m_nbufs(nbufs)
//    {
//        InitializeCriticalSection(&m_BufferPoolLock);
//        InitializeConditionVariable(&m_BuffersAvailable);
//        m_nWaitCount = loadFactor * nbufs;
//        if (initBuffs)
//        {
//            initializeBuffers();
//        }
//    }
//
//    InlineBufferPool::~InlineBufferPool()
//    {
//        DeleteCriticalSection(&m_BufferPoolLock);
//        //TODO: clean up buffers arrays.
//    }
//
//    void InlineBufferPool::initializeBuffers()
//    {
//        for (int i = 0; i < m_nbufs; i++)
//        {
//            Buffer *b = new Buffer();
//            ZeroMemory(b->data, 96*sizeof(Buffer::Datum));
//            m_BufferPool.push(b);
//        }
//    }
//
//    void InlineBufferPool::postBuffer(Buffer *b)
//    {
//        EnterCriticalSection(&m_BufferPoolLock);
//        size_t szpool=m_BufferPool.size();
//        while (szpool > m_nWaitCount || szpool == m_nbufs) 
//        {
//            SleepConditionVariableCS(&m_BuffersAvailable, &m_BufferPoolLock, INFINITE);
//        }
//        m_BufferPool.push(b);
//        WakeConditionVariable(&m_BuffersAvailable);
//        LeaveCriticalSection(&m_BufferPoolLock);
//    }
//
//    Buffer* InlineBufferPool::getBuffer()
//    {
//        EnterCriticalSection(&m_BufferPoolLock);
//        while (m_BufferPool.size() == 0)
//        {
//            SleepConditionVariableCS(&m_BuffersAvailable, &m_BufferPoolLock, INFINITE);
//        }
//        Buffer *front = m_BufferPool.front();
//        m_BufferPool.pop();
//
//        LeaveCriticalSection(&m_BufferPoolLock);
//
//        return front;
//    }
//
//
//}
