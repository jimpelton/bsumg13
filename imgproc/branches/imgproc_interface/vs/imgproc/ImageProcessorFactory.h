

#ifndef _IMAGEPROCESSORFACTORY_H
#define _IMAGEPROCESSORFACTORY_H

//#include "ImageProcessor405.h"
//#include "ImageProcessor485.h"
#include "Centers.h"
#include "WellIndexImageProcessor.h"
#include "CircleDrawingImageProcessor.h"

namespace uG
{
    /**
      *	\brief Creates concrete specializations AbstractImageProcessor.
      *	
      *	A possible change in the future needs to address the string::find usage,
      *	and eliminate it for something else...but this part is still evolving.
      *	
      *	//  [1/21/2013 jim]
      */
    class ImageProcessorFactory
    {
    public:
        static ImageProcessorFactory* getInstance()
        {
            static ImageProcessorFactory *myself = NULL;
            if (myself == NULL)
            {
                myself = new ImageProcessorFactory();
            }
            return myself;
        }

        AbstractImageProcessor* newProc()
        {
            size_t at = uG_IMAGEPROC_TYPE.find("DEBUG");
            if (at != std::string::npos)
                return new CircleDrawingImageProcessor();
            else
                return new WellIndexImageProcessor();
        }

    private:
        ImageProcessorFactory() {} //no constructing for you!

    };
}







#endif  /* _IMAGEPROCESSORFACTORY_H */
