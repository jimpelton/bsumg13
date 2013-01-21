

#ifndef _IMAGEPROCESSORFACTORY_H
#define _IMAGEPROCESSORFACTORY_H

#include "ImageProcessor405.h"
#include "ImageProcessor485.h"

using std::string;

namespace uG
{
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

        AbstractImageProcessor* getProc(const string &fname, 
            unsigned char *data, long long *buf)
        {
            size_t found = fname.find("Camera405");
            if (found != string::npos)
                return new ImageProcessor405(data, buf);
            else
                return new ImageProcessor485(data, buf);
        }

    private:
        ImageProcessorFactory() {}

    };
}
//static ImageProcessorFactory::myself = NULL;







#endif  /* _IMAGEPROCESSORFACTORY_H */
