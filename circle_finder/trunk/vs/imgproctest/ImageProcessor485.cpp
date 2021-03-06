
#include "ImageProcessor485.h"
#include "Centers.h"

#include <math.h>
#include <iostream>

namespace uG
{

    ImageProcessor485::ImageProcessor485(unsigned char *data, long long *buf_values)
        : AbstractImageProcessor(data, buf_values)
    {
        //nothing
    }

    ImageProcessor485::~ImageProcessor485()
    {
        //nothing
    }

    void ImageProcessor485::process()
    {
        for(int row = 0; row<CENTERS_ROW_COUNT; row++)
        {
            for (int col = 0; col<CENTERS_COL_COUNT; col++)
            {
                //pixel start/end
                int wellIdx = (row * CENTERS_COL_COUNT)+col;
                int centerx = static_cast<int>(center485x[row][col]);
                int centery = static_cast<int>(center485y[row][col]);

                int startx = centerx - RADIUS;
                int starty = centery - RADIUS;
                int endx   = centerx + RADIUS;
                int endy   = centery + RADIUS;

                // if (startx<0) startx=0;
                // if (starty<0) starty=0;
                // if (endx>IMAGE_WIDTH)  endx = IMAGE_WIDTH;
                // if (endy>IMAGE_HEIGHT) endy = IMAGE_HEIGHT;

                m_wellValues[wellIdx] = 
                    accumulate(startx, starty, centerx, centery, endx, endy);          
            }
        }
    }

    long long ImageProcessor485::accumulate(int startx, int starty, int centerx, int centery, 
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
