


#include "WellIndexImageProcessor.h"
	
#ifndef CircleDrawingImageProcessor_h__
#define CircleDrawingImageProcessor_h__

namespace uG
{
class CircleDrawingImageProcessor : public WellIndexImageProcessor
{

public:
    CircleDrawingImageProcessor(void);
    virtual ~CircleDrawingImageProcessor(void);

    virtual void process();

protected:
    virtual long long accumulate(int startx, int starty, int centerx, int centery, 
        int endx, int end);
    

};

} // namespace uG

#endif // CircleDrawingImageProcessor_h__
