

#include "Imgproc.h"
#include "Centers.h"

namespace uG
{

    int uG_DEBUG = 0;

    Imgproc::Imgproc()
    {
        initVars();
    }

    Imgproc::~Imgproc(){}

    void Imgproc::initVars()
    {
        m_uGVars.uG_NUM_WELLS = 96;
        m_uGVars.uG_IMAGE_WIDTH=2592;
        m_uGVars.uG_IMAGE_HEIGHT=1944;
        m_uGVars.uG_RADIUS=45;

        m_uGVars.centers = new uGCenter[m_uGVars.uG_NUM_WELLS];
        for (int i = 0; i < m_uGVars.uG_NUM_WELLS; ++i) {
            uGCenter c = { 0.0f, 0.0f, 0.0f };
            m_uGVars.centers[i] = c;
        }
    }



    void Imgproc::addCenter( int idx, double x, double y, double r )
    {
        m_uGVars.centers[idx].x = x;
        m_uGVars.centers[idx].y = y;
        m_uGVars.centers[idx].r = r;
    }

    void Imgproc::setVars(const uGVars *vars)
    {
        m_uGVars = *vars;
    }

} //namepsace uG