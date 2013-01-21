

#ifndef _IMAGEPROCESSOR485_H
#define _IMAGEPROCESSOR485_H

#include "AbstractImageProcessor.h"
#include "Export.h"

namespace uG
{
    class ImageProcessor485 : public AbstractImageProcessor
    {
        friend class ImageProcessorFactory;

    protected:
        virtual long long accumulate(int starty, int startx, int centerx, int centery, 
            int endx, int endy);

        //TODO:change constructor to protected...force factory usage.
    public:
        ImageProcessor485(unsigned char *data, long long *buf_values);

    public:
        virtual ~ImageProcessor485();
        virtual void process();
    };
}

#endif /* _IMAGEPROCESSOR485_H */
