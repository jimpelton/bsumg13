

#ifndef _IMAGEPROCESSORFACTORY_H
#define _IMAGEPROCESSORFACTORY_H

#include "ImageProcessor405.h"
#include "ImageProcessor485.h"
#include "Export.h"

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

        AbstractImageProcessor* newProc(const std::string &bufId, 
            unsigned char *data, long long *buf)
        {
            size_t at = bufId.find("Camera405");
            if (at != std::string::npos)
                return new ImageProcessor405(data, buf);
            else
                return new ImageProcessor485(data, buf);
        }

    private:
        ImageProcessorFactory() {} //no constructing for you!

    };
}







#endif  /* _IMAGEPROCESSORFACTORY_H */
