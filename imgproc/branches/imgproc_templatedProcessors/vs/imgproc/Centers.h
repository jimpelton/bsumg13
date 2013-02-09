     
#ifndef Centers_h__
#define Centers_h__

namespace uG
{

/// Represents an x,y point and radius around that point.
/// Used in imgproc to represent the well plate circles.
struct uGCenter 
{
    double x, y, r;
};

//TODO: remove uGVars struct.
struct uGVars 
{
    int uG_NUM_WELLS;
    int uG_IMAGE_WIDTH;
    int uG_IMAGE_HEIGHT;
    int uG_RADIUS;
    uGCenter *centers;
};

/// Contains information needed by an AbstractImageProcessor
struct uGProcVars 
{
    int radius;
    int numWells;
    int imgw;
    int imgh;
    size_t nElements;
    uGCenter *centers;
};

/// 1 if we will draw DEBUG circles, 0 otherwise.
extern int uG_DEBUG;



} /* namespace uG */
#endif /* Centers_h__ */
