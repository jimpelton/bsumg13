/*
 * CLParser.cpp
 *
 *  Created on: Dec 25, 2011
 *      Author: jim
 *
 *  Purpose: A convenienve class for parsing the given command line.
 */

#include "CLParser.h"
#include <cstdlib>
#include <string.h>
#include <sstream>

CLParser *CLParser::myself = NULL;

CLParser::CLParser() : argsmap()
{

}

CLParser::~CLParser()
{
}

void CLParser::CleanUp()
{
	delete myself;
	myself = NULL;
}

/**
 * Initialize the CLParser with the command line in argv.
 * @param argc
 * @param argv
 * @return
 */
int CLParser::Init(int argc, char *argv[])
{
	if (myself == NULL)
	{
		myself = new CLParser();

	}

	int i = 1;
	while (i < argc) {
		std::string arg(argv[i]); 
        if (arg[0] == '-') {
            //arg starts with at least one "-"
            char *argval;
            if (arg[1] == '-') {  
                //arg starts with "--"
                if (i+1 < argc) {
                    //get the next arg.
                    i+=1;
                    argval = argv[i];
                    if (argv[i][0] == '-') {  
                        //we have an option-less argument
                        argval = "FLAG";
                        i-=1;
                    }
                    myself->argsmap[arg[2]] = argval;
                } else {
                    next = "FLAG";
                    myself->argsmap[title.substr(2)] = next;
                    return i;
                }
                i+=1;
		    } else { //short option
                std::string next;
                if (i+1 < argc)
                {
                    i+=1;
                    next = argv[i];
                    if (next.substr(0,2).compare("--") == 0 || next.substr(0,1).compare("-") == 0) {  //we have an option-less argument
                        next = "FLAG";
                        i--;
                    }
                    myself->argsmap[title.substr(1)] = next;
                }else{
                    next = "FLAG";
                    myself->argsmap[title.substr(1)] = next;
                    return i;
                }
                i+=1;
        }

	}//while
	return i;
}

/**
 *
 * @param src
 * @param dst
 * @return
 */
bool CLParser::ParseCL_n(const char * src, int * dst)
{
	ArgIter iter = myself->argsmap.find(src);
	if (iter == myself->argsmap.end())
	{
		return false;
	}

	std::istringstream input(iter->second);
	input >> *dst;
	return true;
}

/**
 *
 * @param src
 * @param dst
 * @return
 */
bool CLParser::ParseCL_s(const char *src, char ** dst)
{
	ArgIter iter = myself->argsmap.find(src);
	if (iter == myself->argsmap.end())
	{
		return false;
	}

	std::string *val = &(iter->second);
	if (val != NULL){
		*dst = (char*) malloc(sizeof(char) * val->length());
		strcpy(*dst, val->c_str());
	}else{
		*dst = NULL;
		return false;
	}

	return true;
}

/**
 *
 * @param src
 * @param dst
 * @return
 */
bool CLParser::ParseCL_flag(const char *src)
{
	ArgIter iter = myself->argsmap.find(src);
	if (iter == myself->argsmap.end())
	{
		return false;
	}

	if (iter->second.compare("FLAG") != 0)
	{
		return false;
	}

	return true;
}
