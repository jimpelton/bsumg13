
#include "AbstractImageProcessor.h"
#include "ImageProcessorFactory.h"

#include "Imgproc.h"
#include "Centers.h"
#include <string>

using std::string;

namespace uG
{

    int uG_DEBUG = 0;

    Imgproc::Imgproc()
        : m_procVars(NULL)
    {
    }


    Imgproc::Imgproc(int imgw, int imgh, int radius, int nWells, ProcType procType)
    {
        m_procVars = new uG::uGProcVars;

        m_procVars->radius = radius;
        m_procVars->numWells = nWells;
        m_procVars->imgw = imgw;
        m_procVars->imgh = imgh;
        m_procVars->procType = procType;
        m_procVars->centers = new uG::uGCenter[nWells];

    }
        
    Imgproc::~Imgproc()
    {
//        if (m_procVars != NULL)
//            freeProcVars();
    }

    void Imgproc::freeProcVars()
    {


    }


    void Imgproc::addCenter( int idx, double x, double y, double r )
    {
        m_uGVars.centers[idx].x = x;
        m_uGVars.centers[idx].y = y;
        m_uGVars.centers[idx].r = r;
    }



    size_t Imgproc::getBlackCirclesImage(const string &filename, uGProcVars vars,
            unsigned char **buf)
    {

        *buf=NULL;
        long long *bufout = new long long[vars.numWells];

        size_t sz_alloc = Reader::openImage(filename, buf);
        AbstractImageProcessor *aip = 
            ImageProcessorFactory::getInstance()->newProc(&vars);
        aip->setOutput(bufout);
        aip->setInput(*buf);
        aip->process();
        
        delete [] bufout;

        return sz_alloc;
    }


    uGProcVars* Imgproc::getCurrVars() 
    {
        return m_procVars;
    }

    uGProcVars* Imgproc::newProcVars(int nWells)
    {
        if (m_procVars != NULL)
        {
            //free the vars!
        }
        m_procVars = new uG::uGProcVars;
        m_procVars->centers = new uG::uGCenter[nWells];
        return m_procVars;
    }


//    void Imgproc::setVars(const uGVars *vars)
//    {
//        m_uGVars = *vars;
//    }


//    void Imgproc::initVars()
//    {
//        m_uGVars.uG_NUM_WELLS = 96;
//        m_uGVars.uG_IMAGE_WIDTH=2592;
//        m_uGVars.uG_IMAGE_HEIGHT=1944;
//        m_uGVars.uG_RADIUS=45;
//
//        m_uGVars.centers = new uGCenter[m_uGVars.uG_NUM_WELLS];
//        for (int i = 0; i < m_uGVars.uG_NUM_WELLS; ++i) {
//            uGCenter c = { 0.0f, 0.0f, 0.0f };
//            m_uGVars.centers[i] = c;
//        }
//    }


} //namepsace uG
