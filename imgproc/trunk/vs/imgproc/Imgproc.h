/********************************************************************
	filename: 	E:\Programming\microgravity\imgproc.svn\trunk\imgproc\imgproc\Imgproc.h
	file base:	Imgproc
	author:		jim
	
	purpose:	The main Imgproc interface for client applications.
*********************************************************************/


#ifndef Imgproc_h__
#define Imgproc_h__

namespace uG
{


/**
 *	\brief Provides an interface to the Imgproc system for client applications.
 *	
 */
    //template < class _ImgTy< int _nWellCnt >, class _ProcTy >
    class Imgproc 
    {
    public:
        //_ImgTy< _nWellCnt > *image;
        //_ProcTy *processor;

        Imgproc();
        ~Imgproc();

        ///add center to the centers collection.
        //uGCenter addCenter(double x, double y, double r);
    };

}
#endif // Imgproc_h__