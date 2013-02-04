

#include "AbstractImageProcessor.h"

namespace uG
{

AbstractImageProcessor::AbstractImageProcessor() 
    : m_data(0)
    , m_wellValues(0)
    , m_centers(0)
{
    m_numWells=0;
    m_wellValues=0;
    m_imageHeight=0;
    m_imageWidth=0;
}

AbstractImageProcessor::~AbstractImageProcessor() { /*nothing*/ }

}  /* namespace uG */
