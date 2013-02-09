
#ifndef _ABSTRACTIMAGEPROCESSOR_H
#define _ABSTRACTIMAGEPROCESSOR_H

#include "Centers.h"

namespace uG
{

typedef unsigned short raw_val_t;

/**
  *	\brief Abstract base class for all ImageProcessor types.
  *	\see ImageProcessorFactory
  *	
  *	Used by Processor classes to reduce large image data to smaller 
  *	meaningful values.
 */
class AbstractImageProcessor
{
protected:
    /**
    *  \param data The large image array to be processed.
    *  \param buf_values The result of processing.
    *  
    *  Typically data and buf_values come from a BufferPool Buffer object.
    */
    AbstractImageProcessor();

    ///Ptr to source data
    const unsigned char *m_data; 

    ///Ptr to output data.
    long long *m_wellValues;

    /// Array of centers, length = m_wellValues.
    const uGCenter *m_centers;

    int m_numWells;
    int m_wellRadius;
    int m_imageWidth; 
    int m_imageHeight;

public:
    virtual ~AbstractImageProcessor();

    /**
    * \brief Define the processing behavior of the AIP.
    */
    virtual void process() = 0;


/************************************************************************/
/* Accessors                                                            */
/************************************************************************/

    void setInput(const unsigned char *d)    { m_data = d; }
    void setOutput(long long *outdat)        { m_wellValues = outdat; }
    void setCenters(const uGCenter *centers) { m_centers = centers; }
    void setWellRadius(int r)                { m_wellRadius = r; }
    void setNumWells(int nWells)             { m_numWells = nWells; }
    void setImageWidth(int imgw)             { m_imageWidth = imgw; }
    void setImageHeight(int imgh)            { m_imageHeight = imgh; }

    void setVars(uGProcVars *vars)
    {
        this->setCenters(vars->centers);
        this->setWellRadius(vars->radius);
        this->setNumWells(vars->numWells);
        this->setImageWidth(vars->imgw);
        this->setImageHeight(vars->imgh);
    }

};

} /* namespace uG */


#endif /* _ABSTRACTIMAGEPROCESSOR_H */



