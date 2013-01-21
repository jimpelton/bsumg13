
#ifndef Util_h__
#define Util_h__


#include <vector>
#include <string>
#include <boost/filesystem.hpp>

using namespace boost::filesystem;
using std::string;
using std::vector;

typedef std::vector<path> pathVector;
typedef std::vector<string> stringVector;



bool parseArgs(int argc, char **argv);

/** push onto back of svec if p is a regular file */
void isFile(const path &p, const string &extension, stringVector &svec);

/** Get directory contents of dir_path, only include files with given
 *  extension. File names are pushed onto the given vector<string>. */
void getDirEntries(const string &dir_path, 
    const string &extension, stringVector &svec);


#endif // Util_h__