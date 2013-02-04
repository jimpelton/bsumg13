
#include "CirclesFile.h"

#include <Imgproc.h>

#include <iostream>
#include <vector>
#include <string>
#include <sstream>
#include <fstream>
#include <math.h>
#include <algorithm>
#include <regex>

using namespace std;

typedef std::vector<CenterInfo > centerVector;


CirclesFile::CirclesFile()
    : m_filename()
    , m_centers(NULL)
{}

CirclesFile::CirclesFile(string filename)
    : m_filename(filename)
    , m_centers(NULL)
{}

CirclesFile::~CirclesFile() {}


void CirclesFile::findRows(centerVector &centers, std::vector<centerVector > &rows, 
    int dist_thresh)
{
    std::sort(centers.begin(), centers.end(), sortCenterByY);
	centerVector::iterator centerIt = centers.begin();
	centerVector::iterator centerIt_end = centers.end()-1;
    int curRow = 0;	
    centerVector *aRow;
    centerVector emptyRow;

    if (rows.size() == 0){ rows.push_back(emptyRow); }
	while (centerIt != centerIt_end) {
        // split into rows, and sort each row by X coord.       
		if ( std::abs( (*centerIt).y - (*(centerIt+1)).y ) > dist_thresh) {
            aRow = &rows[curRow];
			aRow->push_back(*centerIt);
            std::sort(aRow->begin(), aRow->end(), sortCenterByX);

            rows.push_back(emptyRow);
            curRow+=1;
            if ( curRow == rows.size() ) break; //on last row. //never true?
		}
		else { rows[curRow].push_back(*centerIt); }
        ++centerIt;
	}
    rows[curRow].push_back(*centerIt);
    //rows[curRow].push_back(*(centerIt+1));
    aRow = &rows[curRow]; 
    std::sort(aRow->begin(), aRow->end(), sortCenterByX);
}

int CirclesFile::writeCirclesFile(vector<CenterInfo> centers, ImageInfo img)
{
    ofstream file(m_filename, std::ios::out);
    if (!file.is_open()) {
        std::cerr << m_filename << " never opened for output! Can't write circles file." << std::endl;
        return -1;
    }

    //initial "header" stuff
    stringstream fileText;
    fileText << "[imgx]:" << img.xdim  << "\n" <<    //image x dim
        "[imgy]:" << img.ydim << "\n" <<             //image y dim
        "[crad]:" << centers.front().r << "\n";      //circle radius

    //sort circles file.
    vector<vector<CenterInfo> > rows;
    findRows(centers, rows, 60);

    int i=0;
    vector<centerVector >::const_iterator rowsEnd = rows.cend();
    for ( vector<centerVector >::const_iterator row = rows.cbegin(); row!=rowsEnd; ++row )
    {
        centerVector::const_iterator rowend = row->cend();
        for (centerVector::const_iterator c = row->cbegin(); c != rowend; ++c )
        {
            fileText << "[" << i << "]:" <<
                c->x << ',' << c->y << '\n';
            i+=1;
        }
    }

    file << fileText.str();
    file.close(); 

    return i;
}

//return -1 on error, >=0 on success.
int CirclesFile::parseCirclesFile()
{

    ifstream file(m_filename, std::ios::in);
    if (!file.is_open()) {
        std::cerr << "Couldn't open given circles file: " << m_filename << std::endl;
        return -1;
    }

    int nLines = 0, nCirc = 0;

    //[xx]|[abcd]:[xxxx],[xxxx]
    regex reg("^\\[(\\d\\d?|[a-zA-Z]{4})\\]:([0-9]{1,4}),?([0-9]{0,4})$");
    cmatch cm;
    char line[50];
    while (!file.eof() && nLines < 500) { //500 chosen just for sanity.
        file.getline(line, 50);
        regex_match(line, cm, reg);
        string key(cm[1]);
        string val(cm[2]);
        string val2(cm[3]);
         
        if (val2.empty()) { 
            try {
                if (key == "imgx") {  
                    m_info.xdim = std::stoi(val);
                } else if (key == "imgy") {
                    m_info.ydim = std::stoi(val);
                } else if (key == "crad") { 
                    m_radius = std::stoi(val);
                }
            } catch (std::exception &eek) {
                std::cerr << "Error parsing circles file: " << eek.what();
                return -1;
            }
        } else {
            int x,y,k,row,col;
            try {

                x = std::stoi(val);
                std::cout << "x=" << x << std::endl;

                y = std::stoi(val2);
                std::cout << "y=" << y<< std::endl;

                k=std::stoi(key); 
                std::cout << "k=" << k << std::endl;

            } catch (std::invalid_argument eek) {
                std::cerr << "Error parsing circles file." << std::endl;                  
                std::cerr << eek.what();
                return -1;
            } catch (std::out_of_range eek) {
                std::cerr << "Argument in circles file was expected to be an integer: " << std::endl;
                std::cerr << eek.what();
                return -1;
            }
            CenterInfo c = { x, y, m_radius };
            m_centers.push_back(c);
            ++nCirc;
        } //else
    } // while (!file.eof...
    return nCirc;
}

CenterInfo CirclesFile::getCenter(int idx)
{
    if (idx < m_centers.size()) {
        return m_centers[idx];
    } else {
        CenterInfo c = {-1,-1,-1};
        return c;
    }
}

//sort ascending by X
bool CirclesFile::sortCenterByX(CenterInfo &lhs, CenterInfo &rhs)
{
    return  lhs.x < rhs.x;
}

//sort ascending by Y
bool CirclesFile::sortCenterByY(CenterInfo &lhs, CenterInfo &rhs)
{
    return lhs.y < rhs.y;
}
