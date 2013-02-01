     
#ifndef Centers_h__
#define Centers_h__

namespace uG
{

struct uGCenter
{
    double x, y, r;
    //explicit uGCenter(double x=0.0, double y=0.0) : x(x), y(y) {}
};

struct uGVars {
    int uG_NUM_WELLS;
    int uG_IMAGE_WIDTH;
    int uG_IMAGE_HEIGHT;
    int uG_RADIUS;
    uGCenter *centers;
};

struct uGProcVars {
    int radius;
    int numWells;
    int imgw;
    int imgh;
    uGCenter *centers;
};

extern int uG_DEBUG;



} /* namespace uG */
#endif /* Centers_h__ */
