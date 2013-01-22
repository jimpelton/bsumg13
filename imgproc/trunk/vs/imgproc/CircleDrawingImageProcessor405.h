
#ifndef CircleDrawingImageProcessor405_h__
#define CircleDrawingImageProcessor405_h__

#include "ImageProcessor405.h"
#include "Centers.h"
    
namespace uG
{
    /**
      *	\brief Sets a processed pixel to 0 after it has been processed.
      *	\see ImageProcessor405
      *	
      *	Note: modifies original data!
      */
	class CircleDrawingImageProcessor405 : public ImageProcessor405
	{

    public:
	    CircleDrawingImageProcessor405(unsigned char *data, long long *buf_values)
	        : ImageProcessor405(data, buf_values)
	    {
	        //empty
	    }

    protected:
	    virtual long long accumulate( int startx, int starty, int centerx, 
            int centery, int endx, int endy ) 
	    {
	        long long rval = 0;
	        raw_val_t *rawdata = (raw_val_t*) m_data;
	        for (int y = starty; y < endy; y++)
	        {
	            for (int x = startx; x < endx; x++)
	            {
	                int pixelIdx = (y * IMAGE_WIDTH)+x;
	                int curx = x-centerx;
	                int cury = y-centery;
	                int dr = sqrtf( curx*curx + cury*cury );
	                if (dr<RADIUS){
	                    rval += rawdata[pixelIdx];
	                    rawdata[pixelIdx]=0;
	                }
	            }
	        }
	        return rval;
	    }
    private:
	};
} /* namespace uG */
#endif // CircleDrawingImageProcessor405_h__