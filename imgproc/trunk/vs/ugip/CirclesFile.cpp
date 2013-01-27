
#include "CirclesFile.h"

#include <Imgproc.h>

#include <iostream>
#include <vector>
#include <string>
#include <sstream>
#include <fstream>
#include <math.h>
#include <algorithm>

using namespace std;


bool sortCenterByX(CenterInfo &lhs, CenterInfo &rhs)
{
    return  rhs.x < lhs.x;
}

bool sortCenterByY(CenterInfo &lhs, CenterInfo &rhs)
{
    return rhs.y < lhs.y;
}

void findRows(vector<CenterInfo> &centers, 
              vector<vector<CenterInfo > > &rows, 
              int dist_thresh)
{
    typedef vector<CenterInfo> centerVector;


    std::sort(centers.begin(), centers.end(), sortCenterByY);

	centerVector::iterator centerIt = centers.begin();
	centerVector::iterator centerIt_end = centers.end()-1;

    int curRow = 0;	
	while (centerIt != centerIt_end) {
        // split into rows, and sort each row by X coord.       
		if ( std::abs( (*centerIt).y - (*(centerIt+1)).y ) > dist_thresh) 
        {
            centerVector aRow = rows[curRow];
            std::sort(aRow.begin(), aRow.end(), sortCenterByX);
			aRow.push_back(*centerIt);
            curRow+=1;
            if ( curRow == rows.size() ) break; //on last row.
		}
		else { rows[curRow].push_back(*centerIt); }
        ++centerIt;
	}
}

int writeCirclesFile(const string filename, 
    vector<CenterInfo> centers, ImageInfo img)
{
    ofstream file(filename, std::ios::out);
    if (!file.is_open())
    {
        std::cerr << filename << " never opened for output! Can't write circles file." << std::endl;
        return -1;
    }
    stringstream fileText;
    
    //initial header stuff
    fileText << "[imgx]:" << img.xdim  << "\n" <<    //image x dim
        "[imgy]:" << img.ydim << "\n" <<             //image y dim
        "[crad]:" << centers.front().r << "\n";      //circle radius

    //sort circles file.
    vector<vector<CenterInfo> > rows(8);
    findRows(centers, rows, 60);

    int i=0;
    vector<CenterInfo>::const_iterator end = centers.end();
    for (vector<CenterInfo>::const_iterator c = centers.begin(); c!=end; ++c)
    {
        fileText << "[" << i << "]:" <<
             c->x+c->r << ',' << c->y+c->r << '\n';
        i+=1;
    }

    file << fileText.str();
    file.close(); 

    return i;
}

//return -1 on error, >=0 on success.
int parseCirclesFile( string fileName )
{
    ifstream file(fileName, std::ios::in);
    int nLines = 0, nCirc = 0;
    if (!file.is_open()) {
        std::cerr << "Couldn't open give circles file: " << fileName << std::endl;
        return -1;
    }

    //[xx]|[abcd]:[xxxx],[xxxx]
    //QRegExp regex("^\\[(\\d\\d?|[a-zA-Z]{4})\\]:([0-9]{1,4}),?([0-9]{0,4})$");

    char line[50];
    while (!file.eof() && nLines < 500) //500 chosen just for sanity.
    {
        file.getline(line, 50);
        //if (regex.indexIn(s)!=0) {
        //    std::cerr << "Couldn't match with regex!" << std::endl;
        //    break;
        //}
        string key = "";//  regex.cap(1);
        string val = "";//  regex.cap(2);
        string val2 ="";//  regex.cap(3);

        int imgWidth=0, imgHeight=0, radius=1;
        if (val2.empty()) //hmm... seems a bit sketchy...
        {
            try
            {
                if (key == "imgx") {
                    imgWidth = std::stoi(val) ;
                    //if (!ok) { std::cerr << "Couldn't convert val to m_imgWidth." << std::endl; break; }
                } else if (key == "imgy") {
                    imgHeight = std::stoi(val);
                    //if (!ok) { std::cerr  << "Couldn't convert val to m_imgHeight." << std::endl; break; }
                } else if (key == "crad") { 
                    radius = std::stoi(val);
                    //if (!ok) { std::cerr << "Couldn't convert val to m_radius." << std::endl; break; }
                }
            }
            catch (std::exception &eek)
            {
                std::cerr << "Error parsing circles file: " << eek.what();
                return -1;
            }
        }
        else
        {
            int x,y,k,row,col;
            try
            {
                x = std::stoi(val);
                std::cout << "x=" << x << std::endl;

                y = std::stoi(val2);
                std::cout << "y=" << y<< std::endl;

                k=std::stoi(key); 
                std::cout << "k=" << k << std::endl;
            }
            catch (std::invalid_argument eek) 
            {
                std::cerr << "Error parsing circles file." << std::endl;                  
                std::cerr << eek.what();
                return -1;
            }
            catch (std::out_of_range eek)
            {
                std::cerr << "Argument in circles file was expected to be an integer: " << std::endl;
                std::cerr << eek.what();
                return -1;
            }

            //TODO: specify 485/405 array somehow.
            uG::Imgproc::addCenter(k,x,y,radius);
            ++nCirc;
        }
        return nCirc;
    } // while (!file.eof...
    return nLines;
}
