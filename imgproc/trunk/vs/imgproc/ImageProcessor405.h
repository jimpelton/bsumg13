
#ifndef _IMAGEPROCESSOR405_H
#define _IMAGEPROCESSOR405_H

#include "AbstractImageProcessor.h"
#include "Export.h"

namespace uG
{
	class ImageProcessor405 : public AbstractImageProcessor
	{
	    friend class ImageProcessorFactory;
	
	protected:
        /** \brief Accumulate the pixel values between start and end parameters.*/
	    virtual long long accumulate(int startx, int starty, int centerx, int centery, 
	            int endx, int endy);
	
        //TODO:change constructor to protected...force factory usage.
	public:
	    ImageProcessor405(unsigned char *data, long long *buf_values);
	
	public:
	    virtual ~ImageProcessor405();

        /**
          * \brief This version of process() also flips the 405 images because 
          * they are upside down and backwards!!!
          * Aieeeee!!!
          */
	    virtual void process();
	};
}


#endif /* _IMAGEPROCESSOR405_H */
