

#include "Imgproc.h"

#define UG_GLOBALS

#ifdef UG_GLOBALS
#include "Centers.h"
#endif // UG_GLOBALS

namespace uG
{

    int CENTERS_ROW_COUNT=8;
    int CENTERS_COL_COUNT=12;
    int NUM_WELLS = CENTERS_ROW_COUNT * CENTERS_COL_COUNT;

    int IMAGE_WIDTH=2592;
    int IMAGE_HEIGHT=1944;
    int RADIUS=45;

    uGCenter g_center405[96] = { {0.,0.} };
    uGCenter g_center485[96] = { {0.,0.} };


    /************************************************************************/
    /* CLASS DEFINITION                                                     */
    /************************************************************************/

    Imgproc::Imgproc()
    {
        //g_center405 = new uGCenter[NUM_WELLS];
        //g_center485 = new uGCenter[NUM_WELLS];

    }
    Imgproc::~Imgproc(){}

}