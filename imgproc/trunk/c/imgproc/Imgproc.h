/********************************************************************
	filename: 	E:\Programming\microgravity\imgproc.svn\trunk\imgproc\imgproc\Imgproc.h
	file base:	Imgproc
	author:		jim
	
	purpose:	The main Imgproc interface for client applications.
*********************************************************************/

#ifdef _WIN32
#include <Windows.h>
#endif

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
class Imgproc 
{
public:

    Imgproc();
    ~Imgproc();

    ///add center to the centers collection.
    void addCenter(int idx, double x, double y, double r);
    void setVars(const uGVars *);    

private:
    void initVars();

     uGVars m_uGVars;
};

} //namespace uG
#endif // Imgproc_h__
