     
#ifndef _CENTERS_H
#define _CENTERS_H

namespace uG
{

struct uGCenter
{
    double x, y;

    //explicit uGCenter(double x=0.0, double y=0.0) : x(x), y(y) {}
};

//All arrays are 8 row x 12 col arrays
extern int CENTERS_ROW_COUNT;
extern int CENTERS_COL_COUNT;
extern int NUM_WELLS;

extern int IMAGE_WIDTH;
extern int IMAGE_HEIGHT;
extern int RADIUS;

extern uGCenter g_center405[96];
extern uGCenter g_center485[96];

} /* namespace uG */
#endif /* _CENTERS_H */