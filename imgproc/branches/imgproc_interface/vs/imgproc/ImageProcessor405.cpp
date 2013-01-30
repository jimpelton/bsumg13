

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
        for(int row = 0; row<uG_CENTERS_ROW_COUNT; row++)
        {
            for (int col = 0; col<uG_CENTERS_COL_COUNT; col++)
            {
                //pixel start/end
                //reversed wellIdx's
                int wellIdx = 95 - ((row * uG_CENTERS_COL_COUNT)+col);
                uGCenter center = uGcenter405[wellIdx]; //405 wells
                int centerx = center.x;
                int centery = center.y;

                int startx = centerx - uG_RADIUS;
                int starty = centery - uG_RADIUS;
                int endx   = centerx + uG_RADIUS;
                int endy   = centery + uG_RADIUS;

                // if (startx<0) startx=0;
                // if (starty<0) starty=0;
                // if (endx>IMAGE_WIDTH)  endx = IMAGE_WIDTH;
                // if (endy>IMAGE_HEIGHT) endy = IMAGE_HEIGHT;

                m_wellValues[wellIdx] = 
                    accumulate(startx, starty, centerx, centery, endx, endy);          
            }
        }
    }

    //TODO: remove sqrt and compare to RADIUS*RADIUS?
    long long ImageProcessor405::accumulate(int startx, int starty, int centerx, int centery, 
        int endx, int endy)
    {
        long long rval = 0;
        const raw_val_t *rawdata = (const raw_val_t*) m_data;
        for (int y = starty; y < endy; y++)
        {
            for (int x = startx; x < endx; x++)
            {
                int pixelIdx = (y * uG_IMAGE_WIDTH)+x;
                int curx = x-centerx;
                int cury = y-centery;
                //std::cout << curx << "," << cury << std::endl;
                int dr = static_cast<int>(sqrtf( (float)(curx*curx + cury*cury) )); 
                if (dr<uG_RADIUS) rval += rawdata[pixelIdx];
            }
        }
        return rval;
    }

}
