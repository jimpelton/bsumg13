


#ifndef CircleDrawingImageProcessor485_h__
#define CircleDrawingImageProcessor485_h__

#include "ImageProcessor485.h"
#include "Centers.h"

namespace uG
{
    class CircleDrawingImageProcessor485 : public ImageProcessor485
    {
    public:
        CircleDrawingImageProcessor485(unsigned char *data, long long *buf_values)
            : ImageProcessor485(data, buf_values)
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
                    int dr = static_cast<int>(sqrtf( curx*curx + cury*cury ));
                    if (dr<RADIUS) {
                        rval += rawdata[pixelIdx];
                        rawdata[pixelIdx]=0;
                    }
                }
            }
            return rval;       
        }
    };
} /* namespace uG */

#endif // CircleDrawingImageProcessor485_h__
