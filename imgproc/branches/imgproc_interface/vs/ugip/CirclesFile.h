

#ifndef CirclesFile_h__
#define CirclesFile_h__

#include <string>
#include <vector>

using std::vector;
using std::string;

struct CenterInfo
{
    int x,y,r;
};

struct ImageInfo
{
    size_t img_size;
    size_t xdim;
    size_t ydim;
};

bool sortCenterByY(CenterInfo &lhs, CenterInfo &rhs); 
bool sortCenterByX(CenterInfo &lhs, CenterInfo &rhs);
/** 
  * \brief Write out the circles file at filename.
  * The circles are sorted by into rows, then the rows
  * are sorted into columns via findRows(  * ). 
  */
int writeCirclesFile(const string filename,
    vector<CenterInfo> centers, ImageInfo img);

/**
  *	\brief Sort centers into rows, then rows into columns.
  */
void findRows(vector<CenterInfo> &centers, 
              vector<vector<CenterInfo > > &rows, 
              int dist_thresh);

/** 
  * \brief Parse the circles file at filename 
  */
int parseCirclesFile(const string filename);

#endif // CirclesFile_h__



