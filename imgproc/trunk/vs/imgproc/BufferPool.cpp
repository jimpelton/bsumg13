//#include "BufferPool.h"
//#include <Windows.h>
//
//namespace uG
//{
//template < int _nbuf, class _Ty >
//BufferPool< _nbuf, _Ty >::BufferPool(int nbufs, int nMaxWait)
//    : m_nbufs(nbufs)
//    , m_nMaxWait(nMaxWait)
//    , m_stopRequested(false)
//{
//    InitializeCriticalSection(&m_BufferPoolLock);
//    InitializeCriticalSection(&m_FullBufferPoolLock);
//    InitializeConditionVariable(&m_FreeBuffersAvailable);
//    InitializeConditionVariable(&m_FullBuffersAvailable);
//    InitializeConditionVariable(&m_FullBufferPoolOpen);
//
//    for (int i = 0; i<nbufs; i++)
//    {
//        Buffer< _Ty > *b = new Buffer< _Ty >();
//        m_BufferPool.push(b);
//    }
//
//}
//
//template < int _nbuf, class _Ty >
//BufferPool< _nbuf, _Ty >::~BufferPool()
//{
//    DeleteCriticalSection(&m_FullBufferPoolLock);
//    DeleteCriticalSection(&m_BufferPoolLock);    
//    //TODO: clean up buffers arrays.
//}
//
//template < int _nbuf, class _Ty >
//Buffer< _Ty >* BufferPool< _nbuf, _Ty >::getFreeBuffer()
//{
//    EnterCriticalSection(&m_BufferPoolLock);
//    while(m_BufferPool.size() == 0)
//    {
//        SleepConditionVariableCS(&m_FreeBuffersAvailable, &m_BufferPoolLock, INFINITE);
//    }
//
//    Buffer< _Ty > *baaahhh = m_BufferPool.top();
//    m_BufferPool.pop();
//    LeaveCriticalSection(&m_BufferPoolLock);
//    return baaahhh;
//}
//
//template < int _nbuf, class _Ty >
//void BufferPool::< _nbuf, _Ty >returnEmptyBuffer(Buffer< _Ty > *b)
//{
//    EnterCriticalSection(&m_BufferPoolLock);
//    m_BufferPool.push(b);
//    WakeConditionVariable(&m_FreeBuffersAvailable);
//    LeaveCriticalSection(&m_BufferPoolLock);
//
//}
//
//template < int _nbuf, class _Ty >
//void BufferPool< _nbuf, _Ty >::postFullBuffer(Buffer< _Ty > *b)
//{
//    EnterCriticalSection(&m_FullBufferPoolLock);
//    while (m_FullBufferPool.size() == m_nMaxWait)
//    {
//        SleepConditionVariableCS(&m_FullBufferPoolOpen, &m_FullBufferPoolLock, INFINITE);
//    }
//
//    m_FullBufferPool.push(b);
//    WakeConditionVariable(&m_FullBuffersAvailable);
//    LeaveCriticalSection(&m_FullBufferPoolLock);
//
//}
//
//template < int _nbuf, class _Ty >
//Buffer< _Ty >* BufferPool< _nbuf, _Ty >::getFullBuffer()
//{
//    EnterCriticalSection(&m_FullBufferPoolLock);
//    while (m_FullBufferPool.size() == 0)
//    {
//        SleepConditionVariableCS(&m_FullBuffersAvailable, &m_FullBufferPoolLock, INFINITE);
//    }
//    Buffer<_Ty > *front = m_FullBufferPool.front();
//    m_FullBufferPool.pop();
//
//    WakeConditionVariable(&m_FullBufferPoolOpen);
//    LeaveCriticalSection(&m_FullBufferPoolLock);
//
//    return front;
//}
//
//
//// }

