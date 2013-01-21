
#include <iterator>
#include <vector>
#include <algorithm>
#include <iostream>
#include <sstream>
#include <boost/filesystem.hpp>

using namespace boost::filesystem;
using std::string;
using std::vector;

typedef std::vector<path> pathVector;
typedef std::vector<string> stringVector;


namespace uG
{
    void usage()
    {
        std::cout << "Usage: imgproc <in-path1> <in-path2>\n";
    }

    bool parseArgs(int argc, char **argv)
    {
        if (argc < 3)
        {
            usage();
            return false;
        }
        return true;
    }

    /** push onto back of svec if p is a regular file */
    void isFile(const path &p, stringVector &svec)
    {
        if (is_regular_file(p) && !is_directory(p))
        {
            string file = p.string();
            svec.push_back(file);
        }
    }

    void getDirEntries(const std::string &dir_path, stringVector &svec)
    {
        //pathVector v;
        path p(dir_path);
        try
        {
            if (exists(p))
            {
                if (is_regular_file(p))
                    std::cout << p << " is a regular file!" << std::endl;
                else if (is_directory(p))
                {
                    directory_iterator dirIter(p), dirIterEnd;

                    while (dirIter != dirIterEnd)
                    {
                        isFile( *dirIter, svec );
                        ++dirIter;
                    }
                }
                else
                    std::cerr << p << "exists, but is not file or directory...confuzzled.\n";
            }
            else
                std::cerr << p << " does not exist. \n";
        }

        catch(const filesystem_error &ex)
        {
            std::cerr << ex.what() << '\n';
        }
    }

}