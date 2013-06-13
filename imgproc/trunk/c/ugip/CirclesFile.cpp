
#include "CirclesFile.h"

//#include <Imgproc.h>
#include <stdio.h>
#include <iostream>
#include <vector>
#include <string>
#include <sstream>
#include <fstream>
#include <math.h>
#include <algorithm>
#include <stdexcept>
#include <boost/algorithm/string.hpp>

using std::string;
using std::vector;
using namespace boost::algorithm;

typedef CirclesFile::CenterInfo CenterInfo;
typedef std::vector<CenterInfo > centerVector;


CirclesFile::CirclesFile()
    : m_filename()
    , m_centers()
    , m_isOpen(false)
{}

CirclesFile::CirclesFile(string filename)
    : m_filename(filename)
    , m_centers()
    , m_isOpen(false)
{}

CirclesFile::~CirclesFile() {}


void CirclesFile::group(centerVector &centers,
                            centerVector &groupLeft,
                            centerVector &groupRight,
                            int middle)
{
    centerVector::iterator centerIt = centers.begin();
    centerVector::iterator centerIt_end = centers.end()-1;

    for(; centerIt != centerIt_end; ++centerIt) {
        if ( (*centerIt).x < middle ) {
                groupLeft.push_back(*centerIt);
        }
        else {
                groupRight.push_back(*centerIt);
        }
    }
}

void CirclesFile::sortPlate(centerVector &group, vector<centerVector > &groupRows,
                            int dist_thresh)
{
    std::sort(group.begin(), group.end(), sortCenterByY);
    centerVector::iterator centerIt = group.begin();
    //centerVector::iterator centerIt_end = group.end()-1;
    centerVector::iterator centerIt_end = group.end();


    unsigned int curRow = 0;
    centerVector *aRow;
    centerVector emptyRow;

    if (groupRows.size() == 0){ groupRows.push_back(emptyRow); }
    while (centerIt != centerIt_end) {
        // split into groupRows, and sort each row by X coord.
        if ( std::abs( (*centerIt).y - (*(centerIt+1)).y ) > dist_thresh) {
            aRow = &groupRows[curRow];
            aRow->push_back(*centerIt);
            std::sort(aRow->begin(), aRow->end(), sortCenterByX);

            groupRows.push_back(emptyRow);
            curRow+=1;
            if ( curRow == groupRows.size() ) break; //on last row. //never true?
        }
        else { groupRows[curRow].push_back(*centerIt); }
        ++centerIt;
    }
    groupRows[curRow].push_back(*centerIt);

    aRow = &groupRows[curRow];
    std::sort(aRow->begin(), aRow->end(), sortCenterByX);
}

void CirclesFile::findRows(centerVector &centers, vector<centerVector > &rows, 
    int dist_thresh)
{
    std::pair<centerVector::iterator, centerVector::iterator> minMaxPair =
            std::minmax_element(centers.begin(), centers.end(), sortCenterByX);

    int middleX = (int)((minMaxPair.first->x + minMaxPair.second->x) / 2.0);

    centerVector leftPlate, rightPlate;
    vector<centerVector > leftPlateRows, rightPlateRows;

    std::sort(centers.begin(), centers.end(), sortCenterByY);

    group(centers, leftPlate, rightPlate, middleX);

    sortPlate(leftPlate, leftPlateRows, dist_thresh);
    sortPlate(rightPlate, rightPlateRows, dist_thresh);

    vector<centerVector >::iterator left_it = leftPlateRows.begin();
    vector<centerVector >::iterator right_it = rightPlateRows.begin();

    int i = leftPlateRows.size() > rightPlateRows.size() ?
            leftPlateRows.size() : rightPlateRows.size();
    int cnt = 0;

    while(cnt < i)
    {
        centerVector cv(leftPlateRows[cnt].begin(), leftPlateRows[cnt].end());
        cv.insert(cv.end(), rightPlateRows[cnt].begin(), rightPlateRows[cnt].end());
        rows.push_back(cv);
        cnt += 1;
    }


//	centerVector::iterator centerIt = centers.begin();
//	centerVector::iterator centerIt_end = centers.end()-1;
//    unsigned int curRow = 0;
//    centerVector *aRow;
//    centerVector emptyRow;
//
//    if (rows.size() == 0){ rows.push_back(emptyRow); }
//	while (centerIt != centerIt_end) {
//        // split into rows, and sort each row by X coord.
//		if ( std::abs( (*centerIt).y - (*(centerIt+1)).y ) > dist_thresh) {
//            aRow = &rows[curRow];
//			aRow->push_back(*centerIt);
//            std::sort(aRow->begin(), aRow->end(), sortCenterByX);
//
//            rows.push_back(emptyRow);
//            curRow+=1;
//            if ( curRow == rows.size() ) break; //on last row. //never true?
//		}
//		else { rows[curRow].push_back(*centerIt); }
//        ++centerIt;
//	}
//    rows[curRow].push_back(*centerIt);
    //rows[curRow].push_back(*(centerIt+1));
//    aRow = &rows[curRow];
//    std::sort(aRow->begin(), aRow->end(), sortCenterByX);

}

int CirclesFile::writeCirclesFile(vector<CenterInfo> centers, ImageInfo img)
{

    //sort circles file.
    vector<vector<CenterInfo > > rows;
    findRows(centers, rows, 30);

    std::stringstream fileText; 
    writeHeader(img, centers[0].r, fileText);

    //int rval = writeRows(rows, fileText);
    int rval = writeCirclesVector(centers, fileText);
    return rval;
}

int CirclesFile::writeRows(vector<vector<CenterInfo > > &rows,
        std::stringstream &fileText)
{
    std::ofstream file(m_filename.c_str(), std::ios::out);
    if (!file.is_open()) {
        std::cerr << m_filename << " never opened for output! Can't write circles file." << std::endl;
        return -1;
    }

    int i=0;
    vector<centerVector >::const_iterator rowsEnd = rows.cend();
    for ( vector<centerVector >::const_iterator row = rows.cbegin(); row!=rowsEnd; ++row )
    {
        centerVector::const_iterator rowend = row->cend();
        for (centerVector::const_iterator c = row->cbegin(); c != rowend; ++c )
        {
            fileText <<  i << ':' <<
                c->x << ',' << c->y << '\n';
            i+=1;
        }
    }

    file << fileText.str();
    file.close(); 

    return i;

}

int CirclesFile::writeCirclesVector(vector<CenterInfo > &centers, 
        std::stringstream &fileText)
{
    std::ofstream file(m_filename.c_str(), std::ios::out);
    if (!file.is_open()) {
        std::cerr << m_filename << " never opened for output! Can't write circles file." << std::endl;
        return -1;
    }

    vector<CenterInfo >::const_iterator it = centers.cbegin();
    vector<CenterInfo >::const_iterator it_end = centers.end();

    int i = 0;
    for(; it != it_end; ++it, ++i)
    {
        fileText << i << ':' <<
            it->x << ',' << it->y << '\n';
    }


    file << fileText.str();
    file.close(); 
    return i;
}

void CirclesFile::writeHeader(ImageInfo img, int rad, std::stringstream &fileText)
{
    fileText << "imgx:" << img.xdim  << "\n" <<    //image x dim
        "imgy:" << img.ydim << "\n" <<             //image y dim
        "crad:" << rad << "\n";      //circle radius
}

//return -1 on error, >=0 on success.
int CirclesFile::open()
{

    std::ifstream file(m_filename.c_str(), std::ios::in);
    if (!file.is_open()) {
        std::cerr << "Couldn't open given circles file: " << m_filename << std::endl;
        m_isOpen = false;
        return -1;
    }

    int nLines = 0;
    char line[50];
    while (!file.eof() && nLines < 500) { //500 chosen just for sanity.
        memset(line, 0, sizeof(line));
        file.getline(line, 50);
        if (p_Line(line, nLines) == 1){
            nLines += 1;
        } else break;
    }
    file.close();

    m_isOpen = true;

   vector<CenterInfo>::iterator it = m_centers.begin();
    for(; it != m_centers.end(); ++it)
    {
        it->r = m_radius;
    }

    return m_centers.size();
}

// parse a line, which is <stuff> ':' <stuff> [','<stuff>]
int CirclesFile::p_Line(string line, int lnum)
{
    int rval=1;
    if (line.empty()) { return rval; }

    trim(line);

    vector<string> splitVals;
    split(splitVals, line, boost::is_any_of(":"));  //split on ':'

    if (splitVals.size() < 2){

        printf("Syntax error in circles file. Is this line incomplete? " \
               "\n\t Line: %d: %s",
               lnum, line.c_str());
        rval = 0;

    } else {

        trim(splitVals[0]);
        trim(splitVals[1]);
        if ( !(rval = p_WellLoc(splitVals[0], splitVals[1], lnum)) ) 
        {
            rval = p_ImgInfo(splitVals[0], splitVals[1], lnum);
        }

    }

    return rval;
}

// Parse a well location, which is <wellIndex> ':' <x>,<y>
// This function splits wellCenter on a ',' and fails if there is no
// comma.
// Returns 0 on success, 1 on failure.
int CirclesFile::p_WellLoc(string wellIdx, string wellCenter, int lnum)
{
    if (wellIdx.empty()) return 0;
    int rval = 1;

    vector<string> splitVals;
    split(splitVals, wellCenter, boost::is_any_of(","));

    if (splitVals.size() < 2){
        rval=0;
    } else {
        int nvalue, n2value, idx;
        try {

            nvalue=stoi(splitVals[0]);
            n2value=stoi(splitVals[1]);
            idx=stoi(wellIdx);

            CenterInfo c(nvalue, n2value);
            m_centers.push_back(c);

        }catch (const std::invalid_argument &eek){
            fprintf(stdout, "Syntax error near line: %d.  Expected a number.\n", lnum);
            rval=0;
        }catch (const std::out_of_range &eek){
            fprintf(stdout, "Syntax error near line: %d. Expected a number.\n", lnum);
            rval=0;
        }
    }
    return rval;
}

int CirclesFile::p_ImgInfo(string key, string value, int lnum)
{
    int rval = 1;
    try {
        if ( key == "imgx") {
            m_imgx = stoi(value);
        } else if ( key == "imgy" ) {
            m_imgy = stoi(value);
        } else if (key == "crad") {
            m_radius = stoi(value);
        } else {
            fprintf(stdout, "Syntax error in config file on line %d: " \
                    "key: %s value: %s.\n", lnum, key.c_str(), value.c_str());
            rval=0;
        }
    } catch (const std::invalid_argument &eek) {
        rval=0;
        fprintf(stdout, "Bad value for integer in config file on line %d, key: " \
                "%s, value: %s.\n", lnum, key.c_str(), value.c_str());
    }
    return rval;
}

CenterInfo CirclesFile::getCenter(int idx)
{
    if (idx < m_centers.size()) {
        return m_centers[idx];
    } else {
        CenterInfo c(-1,-1,-1);  // = {-1,-1,-1};
        return c;
    }
}

//sort ascending by X
bool CirclesFile::sortCenterByX(const CenterInfo &lhs, const CenterInfo &rhs)
{
    return  lhs.x < rhs.x;
}
//sort ascending by Y
bool CirclesFile::sortCenterByY(const CenterInfo &lhs, const CenterInfo &rhs)
{
    return lhs.y < rhs.y;
}
