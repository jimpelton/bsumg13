// playingwithimages.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <iostream>
#include <fstream>

static void EnhanceContrast(char* image, unsigned int width, unsigned int height);
static unsigned int GetAverage(char* image, unsigned int width, unsigned int height);
static unsigned int GetAverage_16(short* image, unsigned int width, unsigned int height);

int _tmain(int argc, _TCHAR* argv[])
{
	const int width = 2592;//should be caps, meh
	const int height= 1944;
	
	unsigned int max = 0;
	unsigned int min = 65536;

	std::ifstream infile405;
	std::ifstream infile485;
	std::ofstream outfile405;
	std::ofstream outfile485;


	char* buffer405 = new char[2*width*height];
	char* buffer485 = new char[2*width*height];
	char filename[256];
	char avg405[256];
	char avg485[256];
	char fileavg405[256];
	char fileavg485[256];

	for(int i = 0; i<20000;i++)
	{
		try
		{
		sprintf_s(filename,256, "Camera405//Camera405_%i.raw", i);
		infile405.open(filename,  std::ifstream::binary);
		sprintf_s(filename,256, "Camera485//Camera485_%i.raw", i);
		infile485.open(filename,  std::ifstream::binary);
		
		infile405.read(buffer405,width*height);
		infile485.read(buffer485,width*height);

		if(infile405.fail())return 0;
		if(infile485.fail())return 0;

		infile405.close();
		infile485.close();

		
		/* flipping the bytes did not increase the values.
		//flip the bytes
		char temp;
		for(int j = 0; j < width*height; j+=2)
		{
			temp = buffer405[j];
			buffer405[j] = buffer405[j+1];
			buffer405[j+1] = temp;

			temp = buffer485[j];
			buffer485[j] = buffer485[j+1];
			buffer485[j+1] = temp;
		}
		*/
/*
		for(int j = 0; j < width*height; j++)
		{
			if(buffer[
		}
		*/

		sprintf_s(avg405,256, "0:%i", GetAverage_16((short*)buffer405,width,height));
		sprintf_s(avg485,256, "0:%i", GetAverage_16((short*)buffer485,width,height));

		sprintf_s(fileavg405,256, "Averages//Average_405_%i.txt", i);
		sprintf_s(fileavg485,256, "Averages//Average_485_%i.txt", i);

		outfile405.open(fileavg405,std::ofstream::out);
		outfile485.open(fileavg485,std::ofstream::out);

		outfile405.write(avg405,strlen(avg405));
		outfile485.write(avg485,strlen(avg485));

		outfile405.close();
		outfile485.close();

		}
		catch(std::ios_base::failure e)
		{
			printf("Something got F'ed in the A at iteration %i. (probably no more files, and is okay)",i);
		}

	}

	delete [] buffer405;
	delete [] buffer485;

	return 0;
}

static void EnhanceContrast(char* image, unsigned int width, unsigned int height)
{
	int pixels = width * height;

	//get average value
	int average = GetAverage(image,width,height);

	//get our new range
	int ihigh = average + (average/2);
	if(ihigh>255)
		ihigh = 255;
	unsigned char high = (unsigned char) ihigh;
	unsigned char low  = (unsigned char) average - (average/2);

	register unsigned int tempval;
	double multiplier = ((double)(high-low)) / 255.0;
	for(int i = 0;i < pixels; i++)
	{   
		tempval =  image[i];
		tempval -= low;
		tempval = (unsigned int) (tempval * multiplier);

		if(tempval > 255)
			tempval = 255;
		image[i]=(unsigned char)tempval;
	}
}

//this works with 8 bit images
static unsigned int GetAverage(char* image, unsigned int width, unsigned int height)
{
	long total=0;
	int pixels = width * height;

	//get total values
	for(int i = 0; i < pixels; i++)
	{
		total+=image[i];
	}
	return (unsigned int) total/pixels;
}

//this works with 16 bit images (assuming the buffer is set up right)
static unsigned int GetAverage_16(short* image, unsigned int width, unsigned int height)
{
	long long total=0;
	int pixels = width * height;

	//get total values
	for(int i = 0; i < pixels; i++)
	{
		total+=image[i];
	}
	return (unsigned int) total/pixels;
}