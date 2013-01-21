
#include "LinearFit.h"

#include <assert.h>
#include <iostream>

namespace uG
{

	LinearFit::LinearFit(const vector<double> &x, const vector<double> &y)
		: m_VecX(x)
		, m_VecY(y)
	{
		assert(x.size() == y.size());
		n = x.size();
		sumX = 0.; 
		sumY = 0.;
		m = 0.;
		b = 0.;
		Sxy = 0.;
		Sxx = 0.;
	}


	LinearFit::~LinearFit(void)
	{
	}

	//! ab[0]=m, ab[1]=b in y=mx+b.
	void LinearFit::linearFit(double *ab)
	{
		compute_sums();
		
		double m = Sxy/Sxx;   //TODO: check for overflow on m.
		double b = (sumY - m*sumX)/n;  //mean(y) - m*mean(x)
		
		std::cout << "m: " << m << " b: " << b << std::endl;

		ab[0]=m; ab[1]=b;
	}

	void LinearFit::compute_sums()
	{
		double sumX2=0, sumXY=0;

		for (size_t i = 0; i < n; ++i)
		{
			double x = m_VecX[i];
			double y = m_VecY[i];

			sumX2 += x*x;
			sumXY += y*x;
			sumX += x;
			sumY += y;
		}
		Sxx = sumX2 - ((sumX*sumX)/n);
		Sxy = sumXY - ((sumX * sumY)/n);
	}




	

}
