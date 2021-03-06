

#include "ImageProcessor405.h"
#include "Centers.h"

#include <math.h>
#include <iostream>

namespace uG
{

    ImageProcessor405::ImageProcessor405(unsigned char *data, long long *buf)
        : AbstractImageProcessor(data, buf)
    {
        //nothing
    }

    ImageProcessor405::~ImageProcessor405()
    {
        //nothing
    }

    void ImageProcessor405::process()
    {
        for(int row = 0; row<CENTERS_ROW_COUNT; row++)
        {
            for (int col = 0; col<CENTERS_COL_COUNT; col++)
            {
                //pixel start/end
                //reversed wellIdx's
                int wellIdx = 96 - ((row * CENTERS_COL_COUNT)+col);
                int centerx = static_cast<int>(center405x[row][col]);
                int centery = static_cast<int>(center405y[row][col]);

                int startx = centerx - RADIUS;
                int starty = centery - RADIUS;
                int endx   = centerx + RADIUS;
                int endy   = centery + RADIUS;

                // if (startx<0) startx=0;
                // if (starty<0) starty=0;
                // if (endx>IMAGE_WIDTH)  endx = IMAGE_WIDTH;
                // if (endy>IMAGE_HEIGHT) endy = IMAGE_HEIGHT;

                m_wellValues[wellIdx-1] = 
                    accumulate(startx, starty, centerx, centery, endx, endy);          
            }
        }
    }

    long long ImageProcessor405::accumulate(int startx, int starty, int centerx, int centery, 
        int endx, int endy)
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
                int dr = sqrtf(curx*curx + cury*cury);
                if (dr<RADIUS) {
                    rval += rawdata[pixelIdx];
                    rawdata[pixelIdx] = 0;
                }
            }
        }
        return rval;
    }

}
