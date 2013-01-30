
#ifndef _ABSTRACTIMAGEPROCESSOR_H
#define _ABSTRACTIMAGEPROCESSOR_H


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
        AbstractImageProcessor(unsigned char *data, long long *buf_values);
        
        ///Ptr to source data
        const unsigned char *m_data; 
        
        ///Ptr to output data.
        long long *m_wellValues;

    public:
        virtual ~AbstractImageProcessor();

        /**
          * \brief Define the processing behavior of the AIP.
          */
        virtual void process() = 0;

        void setInput(const unsigned char *d) { m_data = d; }

        void setOutput(long long *outdat) { m_wellValues = outdat; }

    };

} /* namespace uG */


#endif /* _ABSTRACTIMAGEPROCESSOR_H */



