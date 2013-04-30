#include "CircleDrawingImageProcessor.h"
#include "math.h"

namespace uG {

CircleDrawingImageProcessor::CircleDrawingImageProcessor(void)
{
}


CircleDrawingImageProcessor::~CircleDrawingImageProcessor(void)
{
}

void CircleDrawingImageProcessor::process()
{
    for (int well=0; well<m_numWells; ++well) {
        uGCenter center = m_centers[well];
        //m_wellRadius = center.r;
        int startx = center.x - m_wellRadius;
        int starty = center.y - m_wellRadius;
        int endx   = center.x + m_wellRadius;
        int endy   = center.y + m_wellRadius;

        // if (startx<0) startx=0;
        // if (starty<0) starty=0;
        // if (endx>IMAGE_WIDTH)  endx = IMAGE_WIDTH;
        // if (endy>IMAGE_HEIGHT) endy = IMAGE_HEIGHT;

        m_wellValues[well] = 
            accumulate(startx, starty, center.x, center.y, endx, endy);          
    }
}

long long CircleDrawingImageProcessor::accumulate(int startx, int starty, 
        int centerx, int centery, int endx, int endy)
{
    long long rval = 0;
    unsigned short *rawdata = 
       (unsigned short*) const_cast<unsigned char*>(m_data); //eeeeeeeeee!!!!!!

    int nPixels = 1;
    for (int y = starty; y < endy; y++) {
        for (int x = startx; x < endx; x++) {
            int pixelIdx = (y * m_imageWidth)+x;
            int curx = x-centerx;
            int cury = y-centery;
            int dr = static_cast<int>(sqrtf( (float)(curx*curx + cury*cury) ));
            if (dr < m_wellRadius) {
                rval += rawdata[pixelIdx];
                rawdata[pixelIdx] = 0;
                nPixels+=1;
            }
        }
    }
    return rval/nPixels;
}

} /* namespace uG */
