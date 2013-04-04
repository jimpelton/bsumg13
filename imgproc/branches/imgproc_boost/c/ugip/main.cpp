
#include "Util.h"
#include "uigp2.h"
#include "CirclesFile.h"

#include "Centers.h"
#include "Reader.h"
#include "Processor.h"
#include "Writer.h"
#include "WorkerThread.h"
#include "BufferPool.h"
#include "ugTypes.h"

#include <QtWidgets/QMainWindow>
#include <QtCore/QCoreApplication>

#include <vector>
#include <iostream>

using uG::BufferPool;
using uG::Processor;
using uG::Writer;
using uG::Reader;
using uG::WorkerThread;

using std::string;
using std::vector;

const   int    NUM_READERS       = 2;
const   int    NUM_PROCS         = 2;
const   int    NUM_WRITE         = 2;
const  string  file_extension    = ".raw";

extern  int    sidx;                        // starting image index
extern  int    eidx;                        // ending image index
extern string  circlesFileName;
extern string  infile;
extern string  outfile;

int doCL(int, char**);

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
        
    string inpath(infile); // = string(argv[2]);
    string outpath(outfile);//  = string(argv[3]);

    std::cout << "Input directory: "  << inpath.c_str()  << std::endl;
    std::cout << "Output directory: " << outpath.c_str() << std::endl;
    std::cout << "Circles file: "     << circlesFileName << std::endl;

    CirclesFile circlesFile(circlesFileName);
    int numcirc = circlesFile.parseCirclesFile();
    
    if (numcirc < 1) {
        std::cout << "No circles found in the file, or it couldn't be opened.\n";
        return 0;
    }

    //read every .raw filename into fileVec
    stringVector fileVec;
    getDirEntries(inpath, file_extension, fileVec);
    std::cout << "Found " << fileVec.size() << " .raw files.\n";

    //assume file sizes are all same.
    path proces(fileVec[0]);
    uintmax_t szfile = file_size(proces); 
    std::cout << "Assuming size of: " << szfile << " bytes.\n";

    //vector madness!
    vector<stringVector> readerFilesVec(NUM_READERS);
    vector<Reader*> readers;
    vector<Processor*> procs;
    vector<Writer*> writers;
    vector<WorkerThread*> workers;    

    uG::ImageBufferPool imgbp(60, szfile);
    uG::DataBufferPool  datbp(20, numcirc);
    
    //partition file names for Readers
    int i = 0;
    stringVector::iterator it(fileVec.begin());
    while ( it != fileVec.end() ) {
        readerFilesVec.at(i%NUM_READERS).push_back(*it);
        ++it; ++i;
    }
   
    for (int i = 0; i < NUM_READERS; ++i) {
        Reader *r = new Reader(readerFilesVec[i], &imgbp);
        readers.push_back(r);
        workers.push_back(new WorkerThread(r->do_work, (void*)r));
    }

    uG::uGProcVars vars;    
    vars.imgh = 1944;
    vars.imgw = 2592;
    vars.radius = circlesFile.getRadius();
    vars.numWells = numcirc;
    vars.centers = new uG::uGCenter[numcirc];
    for (int i = 0; i < numcirc; ++i) {
        CenterInfo ci = circlesFile.getCenter(i);
        uG::uGCenter ugc = {ci.x, ci.y, ci.r};
        vars.centers[i] = ugc;
    }

    for (int i = 0; i < NUM_PROCS; ++i) {
        Processor *p = new Processor(&imgbp, &datbp, &vars);
        procs.push_back(p);
        workers.push_back( new WorkerThread(p->do_work, (void*)p) );
    }

    for (int i = 0; i < NUM_WRITE; ++i) {
        Writer *w = new Writer(outpath, &datbp);
        writers.push_back(w);
        workers.push_back(new WorkerThread(w->do_work, (void*)w));
    }

    std::cout << "Created: " << readers.size() << " reader threads.\n";
    std::cout << "Created: " << procs.size() << " processor threads.\n";
    std::cout << "Created: " << writers.size() << " writer threads.\n";
    std::cout << "Starting threads..." << std::endl;

    vector<WorkerThread*>::iterator worker_iter = workers.begin();
    for (; worker_iter != workers.end(); ++worker_iter) {
        (*worker_iter)->go();
    }

    worker_iter = workers.begin();
    for(; worker_iter != workers.end(); ++worker_iter) {
        (*worker_iter)->join();
    }

    //TODO: cleanup!! whaaattt?!? I cleanup for no one!  

    return 0;
}

