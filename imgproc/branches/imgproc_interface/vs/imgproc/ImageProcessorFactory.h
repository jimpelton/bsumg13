

#ifndef _IMAGEPROCESSORFACTORY_H
#define _IMAGEPROCESSORFACTORY_H

#include "Centers.h"
#include "AbstractImageProcessor.h"
#include "WellIndexImageProcessor.h"
#include "CircleDrawingImageProcessor.h"

#include <string>

//class WellIndexImageProcessor;
//class CircleDrawingImageProcessor;

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

        AbstractImageProcessor* newProc(uGProcVars *vars)
        {
            AbstractImageProcessor *rval = NULL;
            if (uG_DEBUG){
                rval = new CircleDrawingImageProcessor();
            } else {
                rval = new WellIndexImageProcessor();
            }

            rval->setVars(vars);
            return rval;
        }

    private:
        ImageProcessorFactory() {} //no constructing for you!

    };
}







#endif  /* _IMAGEPROCESSORFACTORY_H */
