
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
  *	meaningful values.  *	  *  Typically data and buf_values come from a BufferPool Buffer object.
  */
template < class _InTy,  class _OutTy>
class AbstractImageProcessor
{
protected:
    /**
    *  An AIP should be written so that any instance can be reused without
    *  having to be recreated. The data pointers and output pointers can 
    *  be reset and process called again.
    *  //  [2/8/2013 jim] 
    */
    AbstractImageProcessor();

    /// Ptr to source data
    _InTy *m_data; 

    /// Ptr to output data.
    _OutTy *m_wellValues;

    /// Array of centers, length = m_wellValues.
    const uGCenter *m_centers;

    int m_numWells;
    int m_wellRadius;
    int m_imageWidth; 
    int m_imageHeight;

    size_t m_dataElements;

public:
    virtual ~AbstractImageProcessor();

    /**
    * \brief Define the processing behavior of the AIP.
    */
    virtual void process() = 0;


/************************************************************************/
/* Accessors                                                            */
/************************************************************************/

    void setInput(const _InTy *indat);
    void setOutput(_OutTy *outdat);

    void setCenters(const uGCenter *centers) { m_centers = centers; }
    void setWellRadius(int r)                { m_wellRadius = r; }
    void setNumWells(int nWells)             { m_numWells = nWells; }
    void setImageWidth(int imgw)             { m_imageWidth = imgw; }
    void setImageHeight(int imgh)            { m_imageHeight = imgh; }
    void setDataElements(size_t nEle)        { m_dataElements = nEle; }

    void setVars(uGProcVars *vars)
    {
        this->setCenters(vars->centers);
        this->setWellRadius(vars->radius);
        this->setNumWells(vars->numWells);
        this->setImageWidth(vars->imgw);
        this->setImageHeight(vars->imgh);
        this->setDataElements(vars->nElements);
    }

};

template < class _InTy, class _OutTy >
AbstractImageProcessor< _InTy, _OutTy >::AbstractImageProcessor() 
    : m_data(0)
    , m_wellValues(0)
    , m_centers(0)
{
    m_numWells=0;
    m_wellValues=0;
    m_imageHeight=0;
    m_imageWidth=0;
}

template < class _InTy, class _OutTy >
AbstractImageProcessor< _InTy, _OutTy>::~AbstractImageProcessor() { /*nothing*/ }

template < class _InTy, class _OutTy >
void AbstractImageProcessor< _InTy, _OutTy >::setInput(const _InTy *d)
{ 
    m_data = d; 
}

template < class _InTy, class _OutTy >
void AbstractImageProcessor< _InTy, _OutTy >::setOutput(_OutTy *outdat) 
{ 
    m_wellValues = outdat; 
}

} /* namespace uG */


#endif /* _ABSTRACTIMAGEPROCESSOR_H */



