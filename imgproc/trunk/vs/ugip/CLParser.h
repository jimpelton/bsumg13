/*
 * CLParser.h
 *
 *  Created on: Dec 25, 2011
 *      Author: jim
 */

#ifndef CLPARSER_H_
#define CLPARSER_H_

#include <map>
#include <string>


class CLParser {
private:
	static CLParser *myself;

	std::map<std::string, std::string> args;

	void parse();

private:
	CLParser();
	~CLParser();

	typedef std::map<std::string, std::string>::iterator ArgIter;

public:
	static int Init(int, char**);
	static void CleanUp();


	static bool ParseCL_n(const char * src , int* dst);
	//static bool ParseCL_f(const char* src, float* dst);
	static bool ParseCL_s(const char * src, char ** dst);
	static bool ParseCL_flag(const char * src);
	//static bool ParseCL_n_lst(const char *src, int *dst);





};

#endif /* CLPARSER_H_ */
