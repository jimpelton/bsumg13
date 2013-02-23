
#include "Reader.h"
#include "ugTypes.h"



#include <fstream>
#include <string>
#include <vector>

namespace uG{

using std::string;
using std::ifstream;
using std::vector;

Reader::Reader(const vector<string> &fileNames, ImageBufferPool *imagePool)
    : m_rawFile(fileNames)
    , m_imagePool(imagePool) 
    , m_stopRequested(false)
{ }

Reader::~Reader(void) 
{ }

void Reader::operator()() 
{
    std::cout << "Enter reader work method." << std::endl;
    //Reader *me = static_cast<Reader*>(aReader);
    
    vector<string>::iterator it = m_rawFile.begin();
    vector<string>::iterator itEnd = m_rawFile.end();

    while (it != itEnd) {
        m_mutex.lock();
            if (m_stopRequested) break;
        m_mutex.unlock();

        ImageBuffer *imgbuf = m_imagePool->getFreeBuffer();

        imgbuf->id = it->substr(it->find_last_of('/')+1);

        if ( !(openImage(*it, &imgbuf->data)) ) {
            return; //TODO: deliver error?
        }

        m_imagePool->postFullBuffer(imgbuf);
   //     std::cout << me->m_tid << ": Reader posted full buffer." << std::endl;
        ++it;
    }
}

//It is a bug that offset could be greater than the ImageBuffer's 
// size (see Reader::do_work()). This will possibly cause a segfault 
// when read() tries to write past the end of the buffer.
// A possible solution is to pass in the ImageBuffer's data size and compare
// with the file size from tellg() and use the smaller of the two. Also,
// log the occurrance of the condition.
//
// TODO: fix offset larger than image buffer size bug (check that offset<=buffer size).
size_t Reader::openImage( const string &fname, unsigned char **rawData )
{
    ifstream inf(fname.c_str(), std::ios::in | std::ios::binary);

    if (!inf.is_open()) return -1;

    inf.seekg(0, std::ios::end);
    size_t offset = inf.tellg();
    inf.seekg(0, std::ios::beg);

    if (*rawData == NULL)  //TODO: this entire ify-poo can be removed I think, unless planning for use outside of this class.
    {
        *rawData = new unsigned char[offset];
    }
    //ZeroMemory(*rawData, offset); 
    memset(*rawData, 0, offset);

    inf.read((char*)*rawData, offset);

    inf.close();
    return offset;
}

} /* namepsace uG */
