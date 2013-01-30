     
#ifndef _CENTERS_H
#define _CENTERS_H

namespace uG
{

struct uGCenter
{
    double x, y, r;

    //explicit uGCenter(double x=0.0, double y=0.0) : x(x), y(y) {}
};

//All arrays are 8 row x 12 col arrays
extern int uG_CENTERS_ROW_COUNT;
extern int uG_CENTERS_COL_COUNT;
extern int uG_NUM_WELLS;

extern int uG_IMAGE_WIDTH;
extern int uG_IMAGE_HEIGHT;
extern int uG_RADIUS;

extern uGCenter uGcenter405[96];
extern uGCenter uGcenter485[96];

} /* namespace uG */
#endif /* _CENTERS_H */