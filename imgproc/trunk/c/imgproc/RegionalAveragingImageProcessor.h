

#ifndef RegionalAveragingImageProcessor_h__
#define RegionalAveragingImageProcessor_h__

#include "AbstractImageProcessor.h"

namespace uG 
{

class RegionalAveragingImageProcessor : public AbstractImageProcessor
{
public:

    RegionalAveragingImageProcessor()
        : AbstractImageProcessor()   
    {}

    virtual ~RegionalAveragingImageProcessor() {}
   

    virtual void process()
    {
        m_upperLeft = {0,0};
        m_lowerRight = { 2592, 1944 };


        int nPixels=1;
        long long accum = 0;
        for (int y = m_upperLeft.y; y<m_lowerRight.y; ++y){
            for (int x = m_upperLeft.x; x<m_lowerRight.x; ++x){
                int idx = (y*m_imageWidth)+x;
                accum += m_data[idx]; 
                nPixels+=1;
            }
        }
        m_wellValues[0] = accum / nPixels;
    }

    void setUpperLeft(int x, int y)
    {
        m_upperLeft.x = x;
        m_upperLeft.y = y;
    }

    void setLowerRight(int x, int y)
    {
        m_lowerRight.x = x;
        m_lowerRight.y = y;
    }

private:
    struct Point
    {
        int x,y;
    };

    Point m_upperLeft;
    Point m_lowerRight;
    
    



};


}


#endif /*  RegionalAveragingImageProcessor_h__ */






