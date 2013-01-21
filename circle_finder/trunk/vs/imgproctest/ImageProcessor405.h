
#ifndef _IMAGEPROCESSOR405_H
#define _IMAGEPROCESSOR405_H

#include "AbstractImageProcessor.h"

namespace uG
{
	class ImageProcessor405 : public AbstractImageProcessor
	{
	    friend class ImageProcessorFactory;
	
	private:
	    long long accumulate(int starty, int startx, int centerx, int centery, 
	            int endx, int endy);
	
	protected:
	    ImageProcessor405(unsigned char *data, long long *buf_values);
	
	public:
	    virtual ~ImageProcessor405();
	    void process();
	};
}


#endif /* _IMAGEPROCESSOR405_H */
