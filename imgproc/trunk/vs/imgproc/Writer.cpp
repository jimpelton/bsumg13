

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
    {
        InitializeCriticalSection(&m_criticalSection);
    }

    Writer::~Writer()
    {
        DeleteCriticalSection(&m_criticalSection);
    }

    void Writer::writeFile(DataBuffer *buf)
    {
        stringstream ss;

        ss << m_outPath << "/Data" << buf->id << ".txt";

        string outfile = ss.str();
        ofstream ofile(outfile, std::ios::out);

        if (!ofile.is_open())
            cerr << outfile << " never opened for output! Can't write output file!" << endl;

        stringstream filetext;
        for(int i = 0; i < 96; i++)
        {
            filetext << i << ":" << buf->data[i] << '\n';
        }
        ofile << filetext.str();
        ofile.close();
    }

    DWORD WINAPI Writer::do_work(LPVOID pargs)
    {
        Writer *me = (Writer *) pargs;
        me->m_tid = GetCurrentThreadId();

        while (true)
        {
            EnterCriticalSection(&(me->m_criticalSection));
                if (me->m_stopRequested) break;
            LeaveCriticalSection(&(me->m_criticalSection));

            DataBuffer *buf = me->m_pool->getFullBuffer();
            me->writeFile(buf);
            me->m_pool->returnEmptyBuffer(buf);
            cout << me->m_tid << " " << buf->id << " Writer returned empty buffer.\n";
        }
        return 0;
    }

}



