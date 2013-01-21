

#ifndef _IMAGEPROCESSOR485_H
#define _IMAGEPROCESSOR485_H

#include "AbstractImageProcessor.h"

namespace uG
{
    class ImageProcessor485 : public AbstractImageProcessor
    {
        friend class ImageProcessorFactory;

    private:
        long long accumulate(int starty, int startx, int centerx, int centery, 
            int endx, int endy);

    protected:
        ImageProcessor485(unsigned char *data, long long *buf_values);

    public:
        virtual ~ImageProcessor485();
        void process();
    };
}

#endif /* _IMAGEPROCESSOR485_H */
