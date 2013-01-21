

#ifndef _ABSTRACTIMAGEPROCESSOR_H
#define _ABSTRACTIMAGEPROCESSOR_H

namespace uG
{

    typedef unsigned short raw_val_t;

    class AbstractImageProcessor
    {
    protected:
        AbstractImageProcessor(unsigned char *data, long long *buf_values);

        const unsigned char *m_data;
        long long *m_wellValues;

    public:
        virtual ~AbstractImageProcessor();
        virtual void process() = 0;
        void setData(const unsigned char *d) { m_data = d; }

    };
} /* namespace uG */


#endif /* _ABSTRACTIMAGEPROCESSOR_H */



