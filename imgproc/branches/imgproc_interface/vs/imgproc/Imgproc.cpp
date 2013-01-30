

#include "Imgproc.h"

#define UG_GLOBALS

#ifdef UG_GLOBALS
#include "Centers.h"
#endif // UG_GLOBALS

namespace uG
{
    int uG_CENTERS_ROW_COUNT=8;
    int uG_CENTERS_COL_COUNT=12;
    int uG_NUM_WELLS = uG_CENTERS_ROW_COUNT * uG_CENTERS_COL_COUNT;

    int uG_IMAGE_WIDTH=2592;
    int uG_IMAGE_HEIGHT=1944;
    int uG_RADIUS=45;

    uGCenter centers[96] = { {0.,0.,0.} };
    
    Imgproc::Imgproc()
    {
        uG_IMAGEPROC_TYPE = "";
    }

    Imgproc::Imgproc(std::string const &_type) 
    {
        uG_IMAGEPROC_TYPE = _type;
    }

    Imgproc::~Imgproc(){}

    void Imgproc::addCenter( int idx, double x, double y, double r )
    {
        centers[idx].x = x;
        centers[idx].y = y;
        centers[idx].r = r;
    }


}