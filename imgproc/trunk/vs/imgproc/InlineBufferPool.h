//#ifndef BUFFERPOOL_H
//#define BUFFERPOOL_H
//
//#include <Windows.h>
//
//#include "Export.h"
//#include "Buffer.h"
//
//#include <queue>
//
//namespace uG
//{
//
//class InlineBufferPool
//{
//public:
//    InlineBufferPool(int _nbufs, float _loadFactor, bool initBuffs);
//    ~InlineBufferPool();
//
//    Buffer* getBuffer();
//    void postBuffer(Buffer*);
//    void initializeBuffers();
//
//private:
//    int m_nbufs;
//    size_t m_nWaitCount;
//    std::queue<Buffer*> m_BufferPool;
//
//    CRITICAL_SECTION m_BufferPoolLock;
//    CONDITION_VARIABLE m_BuffersAvailable;
//
//
//
//};
//
//} /* namespace uG */
//#endif // BUFFERPOOL_H
