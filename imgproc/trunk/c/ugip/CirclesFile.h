

#ifndef CirclesFile_h__
#define CirclesFile_h__

#include <string>
#include <vector>
#include <map>

using std::vector;
using std::string;
using std::map;


/**
  *	\brief Encapsulates a circles file for easy access to the values in the file.
  *
  * A CirclesFile has the following grammar:
  *        line : wellLoc
  *             | imginfo
  *
  *        wellLoc : num ':' num ',' num
  *
  *        imginfo : key ':' key
  *        key  : string
  *             | num
  */
class CirclesFile 
{
public:

/**
  *	\brief Represents a circle center in x,y coordinates, with the
  *        associated radius.
  */
struct CenterInfo
{
    int x,y,r;

    CenterInfo()
        : x(0),y(0),r(0) {}

    CenterInfo(int x, int y)
        : x(x), y(y), r(0) {}

    CenterInfo(int x, int y, int r)
        : x(x), y(y), r(r) {}
};

/**
  *	\brief Represents the size in bytes, xdim and ydim of an image.
  */
struct ImageInfo
{
    size_t img_size;   //< size in bytes
    size_t xdim;       //< pixels in x dimension
    size_t ydim;       //< pixels in y dimension
};

typedef std::vector<CirclesFile::CenterInfo > cVec;
typedef cVec::iterator cVecIt;
typedef cVec::const_iterator cVecCIt;


/*** CIRCLES FILE IMPLEMENTATION ***************************************** */

    CirclesFile();
    CirclesFile(string fileName, int ncols=8, int nrows=12);
    ~CirclesFile();
    //CirclesFile(const CirclesFile &rhs);


    /** 
    * \brief Write out the circles file.
    *
    * The circles are sorted by into rows, then the rows
    * are sorted into columns via findRows().
    *
    * \return -1 on error, else the number of circles written.
    */
    int writeCirclesFile(cVec centers, ImageInfo img);

    /** 
    * \brief Parse the circles file at filename 
	* \return -1 on error, number of circles parsed on success.
    */
    int open();

    /**
      *	\brief Get the CenterInfo at idx.
      *      	
      *	\return a CenterInfo with radius -1 if no circles or idx out of bounds,
      *	otherwise returns the CenterInfo at idx.
      */
    CenterInfo getCenter(int idx);

    int getNumCircles() const { return m_centers.size(); }
    int getRadius() const { return m_radius; }
    int getXdim() const { return m_imgx; }
    int getYdim() const { return m_imgy; }

    bool isOpen() { return m_isOpen; }

private:
    string m_filename;
    cVec m_centers;
    ImageInfo m_info;

    bool m_isOpen;

    int m_radius, m_imgx, m_imgy;
    int m_numCols, m_numRows;

    int parseCirclesFile_helper(int &nCirc);

    int p_Line(string line, int lnum);
    int p_WellLoc(string wellIdx, string wellCenter, int lnum);
    int p_ImgInfo(string key, string value, int lnum);

    void group(cVec & centers,
               cVec & left,
               cVec & right,
               int middleX);

    void sortPlate(cVec &group,
                   vector<cVec > & groupRows,
                   int dist_thresh);

    void dumb_sortPlate(cVec & group,
                        vector<cVec > & groupRows);

    static bool sortCenterByY(const CenterInfo &lhs,
                              const CenterInfo &rhs);

    static bool sortCenterByX(const CenterInfo &lhs,
                              const CenterInfo &rhs);

    // Sort centers into rows, then rows into columns.
    void findRows(cVec & centers,
                  vector<cVec > & rows,
                  int dist_thresh);

    // write 2d vector of rows into fileText and save to file.
//    int writeRows(vector<cVec > &rows, std::stringstream &fileText);

    // write 1d vector of center info into fileText and save to file.
    int writeCirclesVector(cVec &centers, std::stringstream &fileText);

    //write the file header.
    void writeHeader(ImageInfo img, int rad, std::stringstream &fileText);
};
#endif // CirclesFile_h__



