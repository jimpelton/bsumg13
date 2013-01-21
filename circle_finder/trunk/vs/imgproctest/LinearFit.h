
#ifndef LinearFit_h__
#define LinearFit_h__

#include <vector>
using std::vector;


//typedef boost::numeric::ublas::vector<double> boost_dbl_vec;

namespace uG
{

	class LinearFit
	{
	public:
		LinearFit(const vector<double> &x, const vector<double> &y);
		~LinearFit(void);
		
		void linearFit(double *ab);
		

	private:
		vector<double> m_VecX, m_VecY;
		double sumX, sumY, m, b;
		double Sxy, Sxx;
		double n;

		void compute_sums();
	};
}
#endif // LinearFit_h__

