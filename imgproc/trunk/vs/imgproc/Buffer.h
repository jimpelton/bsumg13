


#ifndef Buffer_h__
#define Buffer_h__

#include <string>

namespace uG
{
    /**
    *	 \class Buffer
    *	 \brief A piece of data that is passed around by BufferPool users.
    *	 
    *	 //  [1/18/2013 jim]
    */
    template <class _Ty> class Buffer 
    {
    public:
        
        typedef _Ty* BufferArray;

        _Ty *data;
        size_t nElements;
        std::string id;

       /** 
         * Create a buffer object with nElem elements of type _Ty. 
         * The id is set to the empty string by default. 
         */
        Buffer(size_t nElem, const std::string &_id = std::string()) 
            : nElements(nElem)
            , id(_id) 
        {
            data=new _Ty[nElem];
            memset(data, 0, nElem*sizeof(_Ty));
        }
    };
}

#endif // Buffer_h__