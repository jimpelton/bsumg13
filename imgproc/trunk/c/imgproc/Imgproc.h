/********************************************************************
	filename: 	E:\Programming\microgravity\imgproc.svn\trunk\imgproc\imgproc\Imgproc.h
	file base:	Imgproc
	author:		jim
	
	purpose:	The main Imgproc interface for client applications.
*********************************************************************/

#ifndef Imgproc_h__
#define Imgproc_h__

#ifdef _WIN32
#include <Windows.h>
#endif

#include "Reader.h"
#include "Writer.h"
#include "Processor.h"

#include <string>

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
    Imgproc(int imgw, int imgh, int radius, int nWells, ProcType procType);
    ~Imgproc();
    

    ///add center to the centers collection.
    void addCenter(int idx, double x, double y, double r);
//    void setVars(const uGVars *);    

    size_t getBlackCirclesImage(const std::string &filename, uGProcVars vars, 
            unsigned char **buf);

    uGProcVars* newProcVars(int nWells);
    uGProcVars* getCurrVars();

private:
    void freeProcVars();
    void buildPipeline(Reader&, Processor&, Writer&);

     uGVars m_uGVars;

     uGProcVars * m_procVars;
};

} //namespace uG
#endif // Imgproc_h__
