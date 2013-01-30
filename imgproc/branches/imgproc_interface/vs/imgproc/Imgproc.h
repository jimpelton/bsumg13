/********************************************************************
	filename: 	E:\Programming\microgravity\imgproc.svn\trunk\imgproc\imgproc\Imgproc.h
	file base:	Imgproc
	author:		jim
	
	purpose:	The main Imgproc interface for client applications.
*********************************************************************/

#include "Centers.h"

#include <string>

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
        Imgproc(std::string const &imgType);
        ~Imgproc();

        ///add center to the centers collection.
        static void addCenter(int idx, double x, double y, double r);
        
        template < class _TyMod >
        static void addModule();
    };
template < class _TyMod >
void Imgproc::addModule()
}
#endif // Imgproc_h__