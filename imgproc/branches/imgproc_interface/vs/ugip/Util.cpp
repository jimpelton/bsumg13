#include "Util.h"
#include "Imgproc.h"

#include <boost/program_options.hpp>
namespace po=boost::program_options;

#include <iostream>
#include <iterator>

#include <fstream>
#include <string>


#define cltest(_arg, _test) strcmp(_arg, _test)==0


using std::vector;
using std::string;

const char *EXTENSION = ".raw";

string circlesFile;
string infile;
string outfile;

bool parseArgs( int argc, char **argv )
{
    try {
       
        po::options_description desc("ugip options");
        desc.add_options()
            ("help", "produce help message")
            ("text", "enter command line mode")
            ("circles-file,c", po::value<string>(), "set circles file")
            ("input-dir,i", po::value<string>(), "set input directory")
            ("output-dir,o", po::value<string>(), "set output directory")
        ;

        po::variables_map vm;
        po::store(po::parse_command_line(argc, argv, desc), vm);
        po::notify(vm);

        if (vm.count("help")) {
            std::cout << desc << "\n";
            return false;
        }

        if (vm.count("circles-file")) {
            circlesFile = vm["circles-file"].as<std::string>();
        } else {
            std::cout << "No circles file given.\n";
        }

        if (vm.count("input-dir")) {
            infile = vm["input-dir"].as<std::string>();
        } else {
            std::cout << "No input dir given.\n";
        }

        if (vm.count("output-dir")) {
            outfile = vm["output-dir"].as<std::string>();
        } else {
            std::cout << "No output dir given.\n";
        }

    } catch(std::exception &eek) {
        std::cerr << "error: " << eek.what() << "\n";
        return false;
    } catch(...) {
        std::cerr << "Exception of type confusing was thrown, bailing out! Aieeee!!!\n";
    }

    return true;
}

void isFile( const path &p, const string &ext, stringVector &svec )
{
    if (is_regular_file(p) && !is_directory(p))
    {
        if (extension(p).compare(ext) == 0){
            svec.push_back(p.generic_string());
        }
    }
}

void getDirEntries( const std::string &dir_path, 
    const std::string &ext, stringVector &svec )
{
    path p(dir_path);
    try {
        if (exists(p)) {
            if (is_regular_file(p))
                std::cout << p << " is a regular file!" << std::endl;
            else if (is_directory(p)) {
                directory_iterator dirIter(p), dirIterEnd;

                while (dirIter != dirIterEnd) {
                    isFile( *dirIter, ext, svec );
                    ++dirIter;
                }
            } else
                std::cerr << p << " exists, but is not file or directory...confuzzled.\n";
        } else
            std::cerr << p << " does not exist. \n";
    }
    catch(const filesystem_error &ex) {
        std::cerr << ex.what() << '\n';
    }
}

