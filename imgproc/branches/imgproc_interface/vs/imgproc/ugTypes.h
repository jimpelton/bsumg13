
#ifndef ugTypes_h__
#define ugTypes_h__

#include "Buffer.h"
#include "BufferPool.h"

namespace uG
{
    typedef BufferPool<unsigned char> ImageBufferPool;
    typedef BufferPool<long long> DataBufferPool;

    typedef Buffer<unsigned char> ImageBuffer;
    typedef Buffer<long long> DataBuffer;
}

#endif // ugTypes_h__


