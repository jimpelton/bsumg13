

#ifndef _IMAGEPROCESSORFACTORY_H
#define _IMAGEPROCESSORFACTORY_H

#include "Centers.h"
#include "AbstractImageProcessor.h"
#include "WellIndexImageProcessor.h"
#include "CircleDrawingImageProcessor.h"

#include <string>

namespace uG
{
    /**
      *	\brief Creates concrete specializations AbstractImageProcessor.
      *	
      *	
      */
    class ImageProcessorFactory
    {
    public:
        static ImageProcessorFactory* getInstance()
        {
            static ImageProcessorFactory *myself = NULL;
            if (myself == NULL) {
                myself = new ImageProcessorFactory();
            }
            return myself;
        }

        AbstractImageProcessor* newProc(uGProcVars *vars)
        {
            AbstractImageProcessor *rval = NULL;
            switch (vars->procType){
                case(DEBUG_CIRCLES):
                    rval = new CircleDrawingImageProcessor();
                    break;
                case(WELL_INDEX):
                    rval = new WellIndexImageProcessor();
                    break;
                default: break;
            }

            if (rval != NULL) {
                rval->setVars(vars);
            }

            return rval;
        }

    private:
        ImageProcessorFactory() {} //no constructing for you!

    };
}







#endif  /* _IMAGEPROCESSORFACTORY_H */
