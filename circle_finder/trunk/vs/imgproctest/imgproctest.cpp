
#include <Windows.h>

#include "imgproctest.h"
#include "ImageProcessorFactory.h"
#include "ImageProcessor405.h"
#include "ImageProcessor485.h"
#include "AbstractImageProcessor.h"
#include "Centers.h"
#include "LinearFit.h"

#include <QDebug>
#include <QPixmap>
#include <QImage>
#include <QRgb>
#include <QGraphicsScene>
#include <QGraphicsItem>
#include <QGraphicsView>
#include <QLine>

#include <boost/filesystem.hpp>
#include <boost/lambda/bind.hpp>

#include <cv.h>

#include <fstream>
#include <iostream>
#include <string>
#include <sstream>
#include <vector>
#include <algorithm>
#include <math.h>
#include <time.h>


using std::ifstream;
using std::string;
using std::vector;
using namespace cv;
using namespace boost::filesystem;
using namespace boost::lambda;

typedef vector<Vec3f> v_vec3f;
typedef vector<Vec3f>::iterator v_vec3f_Iter;
typedef vector<Vec3f>::const_iterator v_vec3f_cIter;

typedef vector< v_vec3f > vv_vec3f; 
typedef vv_vec3f::iterator vv_vec3f_Iter;
typedef vv_vec3f::const_iterator vv_vec3f_cIter;

const int X_COORD=0;
const int Y_COORD=1;
const SORT_METHOD SORT_HORIZONTAL=X_COORD;
const SORT_METHOD SORT_VERTICAL=Y_COORD;

imgproctest::imgproctest(QWidget *parent, Qt::WindowFlags flags)
    : QMainWindow(parent, flags)
{
    ui.setupUi(this);

    m_graphicsScenes = new QGraphicsScene*[10];
    m_graphicsViews = new QGraphicsView*[10];
    int x=0;
    for (int i=0; i<10; ++i)
    {
        m_graphicsScenes[i] = new QGraphicsScene(this);
        m_graphicsViews[i] = new QGraphicsView(this);
        m_graphicsViews[i]->setScene(m_graphicsScenes[i]);
        ui.gridLayout->addWidget(m_graphicsViews[i], i/2, x^1);
    }
    
    
    
//    ui.graphicsView->setScene(m_graphicsScene);
}

imgproctest::~imgproctest()
{

}

void 
imgproctest::setargs(int argc, char **argv)
{
    m_argc = argc;
    for (int i = 1; i < argc; i++){
        m_argv.push_back(QString() = QString(argv[i]));
    }
}

void 
imgproctest::go()
{
    //uG::AbstractImageProcessor *aip;
    string directoryString;

    if (m_argv.size() < 1) { 
        std::cerr << "No file name specified, or something." << std::endl;
        return; 
    }
    ui.fileLabel->setText(m_argv[0]);
    directoryString = m_argv[0].toStdString();

    vector<string> files;
    getRandomFileNames(files, directoryString);
    if (files.size() == 0) {
        std::cerr << "No files where found in dir " << 
            directoryString << std::endl;
        return;
    }

    v_vec3f circles;
    vv_vec3f rows_proto(8);
    vector<vv_vec3f > centers;
    vector<Mat*> images;
    unsigned char *data=NULL, *data8bit=NULL;
    Mat *grey, *medgrey;

    //loop through random files and process.
    for ( vector<string>::iterator fileIter = files.begin(); 
        fileIter != files.end(); ++fileIter )
    {
        size_t length=0;
        if (!openImage(*fileIter, &data, &length)) return;

        //covert 16bit values to 8bit.
        data8bit = new unsigned char[length/2];
        unsigned short *dsauce = (unsigned short*) data;
        for (int i = 0; i < length/2; i++) {
            data8bit[i] = (unsigned char) (dsauce[i] >> 8);
        }

        //start processing the images
        grey = new Mat(IMAGE_HEIGHT, IMAGE_WIDTH, CV_8UC1, data8bit);
        medgrey = new Mat(grey->rows, grey->cols, CV_8UC1);

        medianBlur(*grey, *medgrey, 13);
        calculateCircles(circles, *medgrey, IMAGE_WIDTH, IMAGE_HEIGHT);

        vv_vec3f rows = rows_proto;  //copy-assign prototype vector of rows.
        findRows(circles,rows,60);
        centers.push_back(rows);
        images.push_back(medgrey);    

        delete grey; 
        delete [] data;
        data=NULL; 
        grey=medgrey=NULL;

    } /* for ( vector<string>::iter... ) */

    fillMissingCircles(centers, 60);

//  drawEverything(circles, medgrey);

//  delete aip;

}

void 
imgproctest::getRandomFileNames(vector<string> &files, const string &directoryString)
{
    path p(directoryString);
    if (!exists(p)) 
    { 
        std::cerr << "Spec'd directory " << p.string() <<
            " doesn't exist.\n"; 
        return;
    } 
    else if (! is_directory(p)) 
    { 
        std::cerr << "Whaaaat!?! " << p.string() <<
            "!?! That's not a directory!\n"; 
        return; 
    }

    //since I can't figure out how to do random access with the 
    //directory_iterator we have to just copy into a vector. 
    //it takes a long time. deal.
    vector<path> list;
    std::copy(directory_iterator(p), directory_iterator(), 
        back_inserter(list));

    int numfiles = list.size();
    if (numfiles<10) return;

    srand(time(NULL));
    int i=0, maxtries=100;    //100 chosen for sanity.
    while(i<10 && maxtries>0)
    {
        int random = rand() % numfiles;
        path dirent = list[random];

        std::cout << "Testing: " << dirent.string() << std::endl;
        if (!is_directory(dirent) && is_regular_file(dirent)) {
            vector<string>::iterator pos = 
                std::find(files.begin(), files.end(), dirent.string());

            if (pos == files.end()) {
                files.push_back( dirent.string() );
                ++i;
            } 
        } /* if (!is_directory...) */
        
        --maxtries;
    }
}

void
imgproctest::calculateCircles( v_vec3f &circles, Mat &grey, int w, int h )
{
    static int PrewitHorizontal[][3] = {{-1,-1,-1},{0,0,0},{1,1,1}};
    static int PrewitVertical[][3]   = {{-1,0,1},{-1,0,1},{-1,0,1}};
    static int PrewitDiagLeft[][3]   = {{-1,-1,0},{-1,0,1},{0,1,1}};
    static int PrewitDiagRight[][3]  = {{0,-1,-1},{1,0,-1},{1,1,0}};

    Mat destHor(grey.rows, grey.cols, CV_8UC1);
    Mat destVer(grey.rows, grey.cols, CV_8UC1);
    Mat destLeft(grey.rows, grey.cols, CV_8UC1);
    Mat destRight(grey.rows, grey.cols, CV_8UC1);

    filter2D(grey, destHor,  grey.depth(), Mat(3,3,CV_32SC1,PrewitHorizontal));
    filter2D(grey, destVer,  grey.depth(), Mat(3,3,CV_32SC1,PrewitVertical));
    filter2D(grey, destLeft, grey.depth(), Mat(3,3,CV_32SC1,PrewitDiagLeft));
    filter2D(grey, destRight,grey.depth(), Mat(3,3,CV_32SC1,PrewitDiagRight));

    Mat edgeImage = destHor+destVer+destLeft+destRight;
    HoughCircles(edgeImage, circles, CV_HOUGH_GRADIENT, 1, 90, 25, 30, 40, 60);
}

//void
//imgproctest::sortIntoRows(v_vec3f &circles, vv_vec3f &rows)
//{
//    std::sort(circles.begin(), circles.end(), imgproctest::ycomp_v3f);
//    findRows(circles, rows, 60, SORT_VERTICAL);
//}

//return new vector of circles by the given sortfun.
void
imgproctest::findRows(const v_vec3f &circles, vv_vec3f &rows, 
	                   int dist_thresh)
{
    if (rows.size() < 8) { rows.reserve(8); }

    std::sort(circles.begin(), circles.end(), imgproctest::ycomp_v3f);

	v_vec3f_cIter it = circles.begin();
	v_vec3f_cIter itend = circles.end()-1;

    int curRow = 0;	
	while (it != itend) 
	{
        //check for y diff that is large enough to be on a new row        
		if ( fabs( (*it)[Y_COORD] - (*(it+1))[Y_COORD] ) > dist_thresh) 
        {
            v_vec3f aRow = rows[curRow];
            std::sort(aRow.begin(), aRow.end(), xcomp_v3f);
			aRow.push_back(*it);
            curRow+=1;
            if (curRow == rows.size()) break; //on last row.
		}
		else { rows[curRow].push_back(*it); }
        ++it;
	}
}

void imgproctest::fillMissingCircles( vector<vv_vec3f > &centers, int dist_thresh )
{
    vector< vv_vec3f >::iterator cent_iter = centers.begin();
    for (; cent_iter != centers.end(); ++cent_iter)
    {
        vv_vec3f rows = *cent_iter;
        vv_vec3f_Iter rows_iter = rows.begin();
        for (; rows_iter != rows.end(); ++rows_iter)
        {
            v_vec3f row = *rows_iter;
            v_vec3f_Iter row_iter = row.begin();
            for(; row_iter != row.end(); ++row_iter)
            {
                int dx = fabs( (*row_iter)[X_COORD] - (*(row_iter+1))[X_COORD] );
                if ( dx > dist_thresh ) 
                { 
                    int cnt = dx/dist_thresh;
                }
            }
        }
    }
}

void 
imgproctest::drawEverything( v_vec3f &circles, Mat &baseImage )
{
    vector<QLineF> rowLines, colLines;

    QImage edges(baseImage.data, IMAGE_WIDTH, IMAGE_HEIGHT, QImage::Format_Indexed8);
//    m_graphicsScene->addPixmap(QPixmap::fromImage(edges));

    //rowLines = getRowLines(circles);
    //colLines = getColLines(circles);
    drawCircles(circles);
    //drawLines(rowLines);
    //drawLines(colLines);
 //   ui.graphicsView->scale(0.5, 0.5);     

}

void 
imgproctest::drawCircles(const v_vec3f &circles)
{
    int i=0;
    v_vec3f_cIter it = circles.begin();
    for (; it != circles.end(); ++i, ++it)
    {
        int r=50;
        float x = (*it)[X_COORD];
        float y = (*it)[Y_COORD];
        //      m_graphicsScene->addEllipse(x-r, y-r, r*2, r*2, QPen(Qt::green));
        //     QGraphicsTextItem *t = m_graphicsScene->addText(QString::number(i), QFont("Times", 14));
        //   t->setPos(x,y);
    }
}

void 
imgproctest::drawLines( const vector<QLineF> &lines )
{
    vector<QLineF>::const_iterator lineIt = lines.begin();		
    std::cout << "----------------------------\n";
    for(; lineIt != lines.end(); ++lineIt)
    {
        //m_graphicsScene->addLine(*lineIt, QPen(Qt::red));
        std::cout << 
            "x1:" << (*lineIt).x1() << ", y1:" << (*lineIt).y1() << 
            " x2:" << (*lineIt).x2() << ", y2:" << (*lineIt).y2() << 
            std::endl;
    }
}

bool 
imgproctest::openImage(const string &fname, 
                       unsigned char **rawData, size_t *length)
{
    ifstream inf(fname.c_str(), std::ios::in | std::ios::binary);

    if (!inf.is_open()) return false;

    inf.seekg(0, std::ios::end);
    size_t offset = inf.tellg();
    inf.seekg(0, std::ios::beg);

    if (*rawData == NULL) {
        *rawData = new unsigned char[offset];
        *length=offset;
    }

    ZeroMemory(*rawData, *length);

    inf.read((char*) *rawData, *length);

    inf.close();
    return true;
}

//void 
//imgproctest::getLines(v_vec3f &rows, vector<int> &buckets, vector<QLineF> &lines)
//{
//	typedef vector<double>::iterator dbl_iter;
//
//	vector<double> x, y;
//	double x1, x2, y1, y2;
//	double ab[2] = {0.};
//	int row = 0;
//
//	while (row < buckets.size()-1) {
//
//		for (int i = buckets[row]; i < buckets[row+1]; ++i) {
//			x.push_back(rows[i][0]);
//			y.push_back(rows[i][1]);
//			std::cout << row << " :" << i << "   " << rows[i][0] << ", " << rows[i][1] << std::endl;
//		}
//
//		/*uG::LinearFit lf(x,y); 
//		lf.linearFit(ab);*/
//
//		std::pair<dbl_iter, dbl_iter> xpair = 
//			std::minmax_element(x.begin(), x.end());
//
//		x1 = *(xpair.first);
//		x2 = *(xpair.second);
//		y1 = ab[0]*x1 + ab[1];       //y=mx+b;
//		y2 = ab[0]*x2 + ab[1];
//
//		lines.push_back(QLine(x1,y1,x2,y2));
//
//		x.clear(); 
//		y.clear();
//		ab[0] = ab[1] = 0.;
//		++row;
//	}
//}

//vector<QLineF>
//imgproctest::getRowLines(v_vec3f &circles)
//{
//	vector<int> buckets;
//	vector<QLineF> lines;
//	v_vec3f sorted_into_rows = findRows(circles, buckets, 60, imgproctest::sortIntoRows);
//
//	getLines(sorted_into_rows, buckets, lines);
//	
//	return lines;
//}

//vector<QLineF> 
//imgproctest::getColLines(v_vec3f &circles)
//{
//	vector<int> buckets;
//	vector<QLineF> lines;
//	v_vec3f sorted_into_cols = findRows(circles, buckets, 60, imgproctest::sortIntoCols);
//
//	getLines(sorted_into_cols, buckets, lines);
//
//	return lines;
//}


