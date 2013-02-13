#include "AbstractImageProcessor.h"

#ifndef ImageProcessor_h__
#define ImageProcessor_h__

namespace uG {

class WellIndexImageProcessor : public AbstractImageProcessor
{
public:
    WellIndexImageProcessor();
    //WellIndexImageProcessor(unsigned char *data, long long *buf);
    virtual ~WellIndexImageProcessor(void);

    virtual void process();
protected:
    virtual long long accumulate(int startx, int starty, int centerx, int centery, 
        int endx, int end);


};
} /* namespace uG */

#endif // ImageProcessor_h__
