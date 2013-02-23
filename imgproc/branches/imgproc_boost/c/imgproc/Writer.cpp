

#include "Writer.h"
#include "ugTypes.h"


#include <iostream>
#include <fstream>
#include <string>
#include <sstream>

using std::string;
using std::ofstream;
using std::stringstream;
using std::cerr;
using std::cout;
using std::endl;

namespace uG
{

    Writer::Writer(const string &outPath, DataBufferPool *pool)
        : m_outPath(outPath)
        , m_pool(pool)  
        , m_stopRequested(false)
    { }

    Writer::~Writer()
    { }

    void Writer::operator()()
    {
        while (true) {
            m_mutex.lock();
                if (m_stopRequested) break;
            m_mutex.unlock();

            DataBuffer *buf = m_pool->getFullBuffer();
            writeFile(buf);
            m_pool->returnEmptyBuffer(buf);
            //cout << m_tid << " " << buf->id << " Writer returned empty buffer.\n";
        }
    }

    void Writer::writeFile(DataBuffer *buf)
    {
        stringstream ss;

        ss << m_outPath << "/Data" << buf->id << ".txt";

        string outfile = ss.str();
        ofstream ofile(outfile, std::ios::out);

        if (!ofile.is_open()) {
            cerr << outfile << " never opened for output! Can't write output file!" << endl;
            return;
        }

        stringstream filetext;
        for(int i = 0; i < 96; i++) {
            filetext << i << ":" << buf->data[i] << '\n';
        }
        ofile << filetext.str();
        ofile.close();
    }
}



