

#ifndef CirclesFile_h__
#define CirclesFile_h__

#include <string>
#include <vector>
#include <map>

using std::vector;
using std::string;
using std::map;

/**
  *	\brief Represents a circle center in x,y coordinates, with the
  *        associated radius.
  */
struct CenterInfo
{
    int x,y,r;
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


/**
  *	\brief Encapsulates a circles file for easy access to the values in the file.
  */
class CirclesFile 
{

public:
    CirclesFile();
    CirclesFile(string fileName);
    ~CirclesFile();
    //CirclesFile(const CirclesFile &rhs);


    /** 
    * \brief Write out the circles file.
    *
    * The circles are sorted by into rows, then the rows
    * are sorted into columns via findRows(). 
    */
    int writeCirclesFile(vector<CenterInfo> centers, ImageInfo img);

    /** 
    * \brief Parse the circles file at filename 
	* \return -1 on error, number of circles parsed on success.
    */
    int parseCirclesFile();

    /**
      *	\brief Get the CenterInfo at idx.
      *      	
      *	\return a CenterInfo with radius -1 if no circles or idx out of bounds,
      *	otherwise returns the CenterInfo at idx.
      */
    CenterInfo getCenter(int idx);

    int getNumCircles() const { return m_centers.size(); }
    int getRadius() const { return m_radius; }
    int getXdim() const { return m_info.xdim; }
    int getYdim() const { return m_info.ydim; }

private:
    string m_filename;
    vector<CenterInfo> m_centers;
    map<int, CenterInfo> m_centersMap;    
    ImageInfo m_info;
    int m_radius;

    int parseCirclesFile_helper(int &nCirc);

    int p_Line(string line, int lnum);
    int p_WellLoc(string wellIdx, string wellCenter, int lnum);
    int p_ImgInfo(string key, string value, int lnum);

    static bool sortCenterByY(const CenterInfo &lhs, const CenterInfo &rhs); 
    static bool sortCenterByX(const CenterInfo &lhs, const CenterInfo &rhs);

    /**
    *	\brief Sort centers into rows, then rows into columns.
    */
    static void findRows(vector<CenterInfo> &centers, 
        vector<vector<CenterInfo > > &rows, 
        int dist_thresh);
};
#endif // CirclesFile_h__



