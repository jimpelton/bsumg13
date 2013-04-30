     
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
/// Unused.
struct uGVars 
{
    int uG_NUM_WELLS;
    int uG_IMAGE_WIDTH;
    int uG_IMAGE_HEIGHT;
    int uG_RADIUS;
    uGCenter *centers;
};

enum ProcType
{
    DEBUG_CIRCLES,
    WELL_INDEX
};


/// Contains information needed by an AbstractImageProcessor
struct uGProcVars 
{
    int radius;
    int numWells;
    int imgw;
    int imgh;
    ProcType procType;
    uGCenter *centers;
};

/// 1 if we will draw DEBUG circles, 0 otherwise.
/// initialized in Imgproc.cpp
extern int uG_DEBUG;



} /* namespace uG */
#endif /* Centers_h__ */
