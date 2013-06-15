
#include "Util.h"
#include "uigp2.h"
#include "CirclesFile.h"

#include <Imgproc.h>
#include <Centers.h>
#include <Reader.h>
#include <Processor.h>
#include <Writer.h>
#include <BufferPool.h>
#include <ugTypes.h>

#include <QtWidgets/QMainWindow>
#include <QtCore/QCoreApplication>
#include <boost/thread.hpp>

#include <vector>
#include <iostream>
#include <atomic>

using uG::BufferPool;
using uG::Processor;
using uG::Writer;
using uG::Reader;

using std::string;
using std::vector;
using std::atomic;

const   int    NUM_READERS       = 1;
const   int    NUM_PROCS         = 1;
const   int    NUM_WRITE         = 1;
const  string  file_extension    = ".raw";

extern  int    sidx;                        // starting image index
extern  int    eidx;                        // ending image index
extern string  circlesFileName;
extern string  infile;
extern string  outfile;
extern string  imgProcType;

int gNumFiles = 1;

uG::ProcType makePType(string p);
int doCL(int, char**);

//std::atomic_int readerProgress;
//std::atomic_int writerProgress;
//std::atomic_int procesProgress;

//void cb_reader()
//{
//    readerProgress++;
//}

//void cb_writer()
//{
//    writerProgress++;
//}

//void cb_proces()
//{
//    procesProgress++;
//}

//void print_progress()
//{
//    while (true){

//        int rp = readerProgress.load();
//        int wp = writerProgress.load();
//        int pp = writerProgress.load();

//        printf("\r R: %.2f P: %.2f W: %.2f", rp/(float)gNumFiles,
//               wp/(float)gNumFiles, pp/(float)gNumFiles);

//        sleep(1000);
//    }
//}

int main(int argc, char *argv[])
{
    if (argc>1){
        if (strcmp(argv[1], "--text")==0)
            std::cout << "Going into command line mode...\n";
            return doCL(argc, argv);
    }
    QApplication app(argc, argv);
    uigp2 mw;
    mw.show();
    return app.exec();
}

int doCL(int argc, char *argv[])
{
    if (!parseArgs(argc, argv)) return 0;
        
    string inpath(infile); 
    string outpath(outfile);

    std::cout << "Input directory: "  << inpath.c_str()  << std::endl;
    std::cout << "Output directory: " << outpath.c_str() << std::endl;
    std::cout << "Circles file: "     << circlesFileName << std::endl;
    
    CirclesFile circlesFile(circlesFileName);
    int numcirc = circlesFile.open();
        
    //if we are not doing regional average, then check that we have more than
    //0 circles.
    if (makePType(imgProcType) != uG::REG_AVG) {
        if (numcirc < 1 )  {
            std::cout << "No circles found in the file, or it couldn't be opened.\n";
            return 0;
        }
    } else { numcirc = 1; }

    //read every .raw filename into fileVec
    stringVector fileVec;
    getDirEntries(inpath, file_extension, fileVec);
    gNumFiles = fileVec.size();
    std::cout << "Found " << gNumFiles << " .raw files.\n";

    if (fileVec.size() == 0) {
        std::cout << "Exiting due to lack of files to process.\n";
        return 0;
    }

    //assume file sizes are all same.
    path proces(fileVec[0]);
    uintmax_t szfile = file_size(proces); 
    std::cout << "Assuming size of: " << szfile << " bytes.\n";

    //vector madness!
    vector<stringVector> readerFilesVec(NUM_READERS);
    vector<Reader> readers;
    vector<Processor> procs;
    vector<Writer> writers;
    vector<boost::thread> workers;

    uG::ImageBufferPool imgbp(10, szfile);
    uG::DataBufferPool  datbp(10, numcirc);
    
    //partition file names for Readers
    int i = 0;
    stringVector::iterator it(fileVec.begin());
    while ( it != fileVec.end() ) {
        readerFilesVec.at(i%NUM_READERS).push_back(*it);
        ++it; ++i;
    }
    
    //create Processors
    uG::Imgproc img;
    uG::uGProcVars *vars = img.newProcVars(numcirc);
    vars->imgh = 1944;
    vars->imgw = 2592;
    vars->radius = circlesFile.getRadius();
    vars->numWells = numcirc;
    vars->procType = makePType(imgProcType); //uG::WELL_INDEX;
    for (int i = 0; i < numcirc; ++i) {
        CirclesFile::CenterInfo ci = circlesFile.getCenter(i);
        uG::uGCenter ugc = {ci.x, ci.y, ci.r};
        vars->centers[i] = ugc;
    }
    
    std::cout << "Starting threads..." << std::endl;

    Reader r1(readerFilesVec[0], &imgbp);
    boost::thread th_r1(boost::ref(r1));

    Processor p1(&imgbp, &datbp, vars);
    boost::thread th_p1(boost::ref(p1));

    Writer w1(outpath, &datbp);
    boost::thread th_w1(boost::ref(w1));


//    boost::thread th_prog(print_progress);

    th_r1.join();
    th_p1.join();
    th_w1.join();

    //TODO: cleanup!! whaaattt?!? I cleanup for no one!  

    return 0;
}

uG::ProcType makePType(string p)
{
    if (p.compare("debug-circles")     == 0) {
        std::cout << "Using debug-circles processor.\n";
       return uG::DEBUG_CIRCLES; 
    }
    /*else if (p.compare("well-index") == 0) {
        std::cout << "Using well-index processor.\n";
        return uG::WELL_INDEX;    
    }*/
    else if (p.compare("reg-avg")    == 0) {
        std::cout << "Using reg-avg processor.\n";
        return uG::REG_AVG;
    } else {
        std::cout << "Using well-index processor.\n";
        return uG::WELL_INDEX;
    }
}


