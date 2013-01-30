

#include "AbstractImageProcessor.h"

namespace uG
{

AbstractImageProcessor::AbstractImageProcessor() 
    : m_data(0)
    , m_wellValues(0)
{
    //nothing
}

//AbstractImageProcessor::AbstractImageProcessor(unsigned char *data, long long *buf_values)
//    : m_data(data)
//    , m_wellValues(buf_values)
//{
//    //nothing
//}

AbstractImageProcessor::~AbstractImageProcessor()
{
    //nothing
}

}  /* namespace uG */
