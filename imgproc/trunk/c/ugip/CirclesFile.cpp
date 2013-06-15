
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

typedef std::vector<CirclesFile::CenterInfo > cVec;
typedef cVec::iterator cVecIt;
typedef cVec::const_iterator cVecCIt;


CirclesFile::CirclesFile()
    : m_filename()
    , m_centers()
    , m_isOpen(false)
    , m_numCols(8)
    , m_numRows(12)
{}

CirclesFile::CirclesFile(string filename, int ncols, int nrows)
    : m_filename(filename)
    , m_centers()
    , m_isOpen(false)
    , m_numCols(ncols)
    , m_numRows(nrows)
{}

CirclesFile::~CirclesFile() {}


void CirclesFile::group(cVec &centers,
                        cVec &groupLeft,
                        cVec &groupRight,
                        int middle)
{
    cVecIt c = centers.begin();
    cVecIt c_end = centers.end();

    for(; c != c_end; ++c) {
        if ( (*c).x < middle )
            groupLeft.push_back(*c);
        else
            groupRight.push_back(*c);
    }
}


void CirclesFile::dumb_sortPlate(cVec &group, vector<cVec> &groupRows)
{
    std::sort(group.begin(), group.end(), sortCenterByY);
    int rIdx = 0;
    for(auto &row : groupRows) {
        for(int i = 0; i < m_numCols; ++i){
            row.push_back(group[rIdx*m_numCols + i]);    
        }

        std::sort(row.begin(), row.end(), sortCenterByX);
        rIdx += 1;
    }
}

void CirclesFile::sortPlate(cVec &group, vector<cVec > &groupRows,
                            int dist_thresh)
{

    std::sort(group.begin(), group.end(), sortCenterByY);
    cVecIt centerIt = group.begin();
    cVecIt centerIt_end = group.end()-1;

    unsigned int curRow = 0;
    cVec *aRow;
    cVec emptyRow;

    if (groupRows.size() == 0){ groupRows.push_back(emptyRow); }
    while (centerIt != centerIt_end) {
        // split into rows, and sort each row by X coord.
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

void CirclesFile::findRows(cVec &centers, vector<cVec > &rows,
    int dist_thresh)
{
    std::pair<cVecIt, cVecIt> minMaxPair =
            std::minmax_element(centers.begin(), centers.end(), sortCenterByX);

    int middleX = (int)((minMaxPair.first->x + minMaxPair.second->x) / 2.0);

    cVec leftPlate, rightPlate;
    vector<cVec > leftPlateRows, rightPlateRows;

    group(centers, leftPlate, rightPlate, middleX);

    dumb_sortPlate(leftPlate, leftPlateRows);
    dumb_sortPlate(rightPlate, rightPlateRows);

//    vector<cVec >::iterator left_it = leftPlateRows.begin();
//    vector<cVec >::iterator right_it = rightPlateRows.begin();

    int i = leftPlateRows.size() > rightPlateRows.size() ?
            leftPlateRows.size() : rightPlateRows.size();

    int row = 0;
    while(row < i)
    {
        cVec cv(leftPlateRows[row].begin(), leftPlateRows[row].end());
        cv.insert(cv.end(), rightPlateRows[row].begin(), rightPlateRows[row].end());
        rows.push_back(cv);
        row += 1;
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

int CirclesFile::writeCirclesFile(cVec centers, ImageInfo img)
{

    //sort circles file.
    vector<cVec > rows;
    //findRows(centers, rows, 30);

    std::stringstream fileText; 
    writeHeader(img, centers[0].r, fileText);

    //int rval = writeRows(rows, fileText);
    int rval = writeCirclesVector(centers, fileText);
    return rval;
}

//int CirclesFile::writeRows(vector<cVec > &rows,
//        std::stringstream &fileText)
//{
//    std::ofstream file(m_filename.c_str(), std::ios::out);
//    if (!file.is_open()) {
//        std::cerr << m_filename << " never opened for output! Can't write circles file." << std::endl;
//        return -1;
//    }
//
//    int i=0;
//    vector<cVec >::const_iterator row = rows.cbegin();
//    vector<cVec >::const_iterator rowsEnd = rows.cend();
//    for (row = rows.cbegin(); row!=rowsEnd; ++row)
//    {
//        cVecCIt c = row->cbegin();
//        cVecCIt rowend = row->cend();
//        for (; c != rowend; ++c )
//        {
//            fileText << i << ':' << c->x << ',' << c->y << '\n';
//            i+=1;
//        }
//    }
//
//    file << fileText.str();
//    file.close(); 
//
//    return i;
//
//}

int CirclesFile::writeCirclesVector(cVec &centers,
        std::stringstream &fileText)
{
    std::ofstream file(m_filename.c_str(), std::ios::out);
    if (!file.is_open()) {
        std::cerr << m_filename << " never opened for output! Can't write circles file." << std::endl;
        return -1;
    }

    cVecCIt it = centers.cbegin();
    cVecCIt it_end = centers.end();

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

    cVecIt it = m_centers.begin();
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

    if (splitVals.size() < 2) {

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

CirclesFile::CenterInfo CirclesFile::getCenter(int idx)
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
