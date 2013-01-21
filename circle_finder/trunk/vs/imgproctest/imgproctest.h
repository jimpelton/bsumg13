#ifndef IMGPROCTEST_H
#define IMGPROCTEST_H

#include <QMainWindow>
#include <QString>
#include <QVector>
#include <QLineF>
#include <QGraphicsScene>
#include <string>

#include <cv.h>

#include "ui_imgproctest.h"

using std::ifstream;
using std::string;
using std::vector;
using namespace cv;

typedef int SORT_METHOD ;


class imgproctest : public QMainWindow
{
    Q_OBJECT

public:
    imgproctest(QWidget *parent = 0, Qt::WindowFlags flags = 0);

    ~imgproctest();

    void setargs(int argc, char **argv);

    void go();

private:

    Ui::imgproctestClass ui;
    int m_argc;
    QVector<QString> m_argv;
    QGraphicsScene **m_graphicsScenes;
    QGraphicsView **m_graphicsViews;

   void getRandomFileNames(vector<string> &files, 
                           const string &directoryString);

	void drawEverything(vector<Vec3f> &circles, Mat &baseImage);
	
    //! data mat should be median filtered for best results.
    void calculateCircles(vector<Vec3f> &circles, Mat &data, int w, int h);

    /**
     * \fn void findRows( const vector<Vec3f> &circles, vector< vector<Vec3f> > &rows, 
	 *          int dist_thresh);
     * \brief Organize the 1D row of circles into a 2D vector of rows/columns.
     *        Writes the output into rows.
     *        NOTE: rows will be resized up if it doesn't have a size of >=8.
     *
     * \param circles The 1D row to find the circle rows.
     * \param rows The 2D matrix of circles.
     * \param dist_thresh The estimated distance between neighboring circle centers.
     */
    void findRows( const vector<Vec3f> &circles, vector< vector<Vec3f> > &rows, 
		           int dist_thresh);

    /**
    * \fn void fillMissingCircles(vector<vector<Vec3f> > &centers, int dist_thresh);
    * \brief Find any missing centers in the 2D grid given by centers.
    *
    * \param centers The 2D matrix of centers.
    * \param dist_thresh The estimated distance between each center.
    */
    void fillMissingCircles(vector<vector<vector<Vec3f> > > &centers, int dist_thresh);


	void drawCircles(const vector<Vec3f> &circles);

	void drawLines(const vector<QLineF> &lines);
	
	bool openImage(const string &fname, 
		unsigned char **rawData, size_t *length);
	
	/************************************************************************/
	/*    PRIVATE STATIC MEMBERS                                            */
	/************************************************************************/


    static bool 
    xcomp_v3f(Vec3f first, Vec3f second) { return (first[0] < second[0]); }

    static bool 
    ycomp_v3f(Vec3f first, Vec3f second) { return (first[1] < second[1]); }

	static void 
    sortIntoCols(vector<Vec3f> &row) { std::sort(row.begin(), row.end(), imgproctest::xcomp_v3f); }

    static void 
    myprint(int i) { std::cout << i << std::endl; }
};


//void getLines(vector<Vec3f> &circles, 
//	          vector<int> &buckets, 
//	          vector<QLineF> &lines);

//vector<QLineF> getRowLines(vector<Vec3f> &circles);

//vector<QLineF> getColLines(vector<Vec3f> &circles);

#endif // IMGPROCTEST_H


//aip = uG::ImageProcessorFactory::
//	getInstance()->getProc(file.toStdString(), data, bufbuf);
//aip->process();//// 
