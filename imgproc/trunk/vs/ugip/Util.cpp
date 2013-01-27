#include "Util.h"
#include "CLParser.h"
#include "Imgproc.h"

#include <iostream>
#include <fstream>
#include <string>

#define cltest(_arg, _test) strcmp(_arg, _test)==0

using std::vector;
using std::string;

const char *EXTENSION = ".raw";

char *circlesFile;
char *infile;
char *outfile;

bool parseArgs( int argc, char **argv )
{
     CLParser::Init(argc, argv);

    if (CLParser::ParseCL_flag("text"))
    {
        if (CLParser::ParseCL_s("-c", &circlesFile))
            std::cout << "Circles File: " << circlesFile << std::endl;
        if (CLParser::ParseCL_s("-i", &infile))
            std::cout << "Input dir: " << infile << std::endl;
        if (CLParser::ParseCL_s("-o", &outfile))
            std::cout << "Output dir: " << outfile << std::endl;
    }

    if (argc < 8)
    {
        std::cout << "Usage: " << argv[0] << " [-text [-c <circle-file>]] -i <in-path> -o <out-path>\n";
        std::cout << "\t -text Command-line mode.\n";
        std::cout << "\t\t -c <file-name> Circles Files containing...CIRCLES!";
        return false;
    }
    return true;
}

void isFile( const path &p, const string &ext, stringVector &svec )
{
    if (is_regular_file(p) && !is_directory(p))
    {
        if (extension(p).compare(ext) == 0){
            svec.push_back(p.string());
        }
    }
}

void getDirEntries( const std::string &dir_path, 
    const std::string &ext, stringVector &svec )
{
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
                    isFile( *dirIter, ext, svec );
                    ++dirIter;
                }
            }
            else
                std::cerr << p << " exists, but is not file or directory...confuzzled.\n";
        }
        else
            std::cerr << p << " does not exist. \n";
    }

    catch(const filesystem_error &ex)
    {
        std::cerr << ex.what() << '\n';
    }
}

