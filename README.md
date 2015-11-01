# bsumg13
Automatically exported from code.google.com/p/bsumg13

OBLIGATORY DRAGONS AHEAD WARNING FOLLOWS: WARNING: DRAGONS AHEAD! 

This code is reasearch quality software! It was written to provide an extensible framework for
interfacing with an arbitrary number of sensors and recording data from them. Efforts were
made to keep the flightsoft code matintainable and extensible so that future uG projects
could reuse the framework for any set of sensors that provided C#/.NET SDKs. 
But, as all research projects go, there is never enough time to refine anything except your writing, 
and even then that may not happen!

## A note to future uG teams:
To future uG teams: Feel free to use this code and improve it--it could be so much better, if only we had
known what we know now!

Good luck, and may a trip to Houston be in your future!


## Contents
bsumg13 contains the in-flight capture and post-flight data processing
software used by the 2013 BSU Microgravity team. 
This repository can be found at: 

    http://github.com/jimpelton/bsumg13

The original repository was hosted at code.google.com as an svn repo. When
Google anounced the demise of code.google.com, the repo was auto-exported to
GitHub. This is why this github dump is structured like a classic svn repository 
with branch, tags and truck folders at the top of each project folder.

The 2014 team did a few commits into some 
of the ``flightsoft'' project branches before they formed their own repo, 
but they thoughtfully kept their commits confined to some branches. The 
2013 flightsoft code has remained untouched since Dan did a final commit 
on June 2, 2013.

## Structure
capture and circle_finder were abandoned early on during the project.
The two projects used during the experiment are:

  1. flightsoft: in-flight software used during both flight days.
  2. imgproc: post-flight data processing.

### flightsoft directory structure
```
|flightsoft
|-trunk
|---FlightSoftTest       Tests for flight software
|-----MidlibConsoleTest  
|-----MidlibFormTest
|-----captureTest        Tests for capture software.
|---Midlib               Midlib library for interfacing with cameras.
|---capture              Source for in-flight software.
|---clui                 A command line UI for the capture software.
|---docs 
|---gui                  The in-flight touch UI for capture software.
|---phidget21-windevel   Phidgets SDK for the phidgets sensors used.
```

### imgproc directory structure
```
|imgproc
|-trunk
|---bash                  Bash scripts for automated conversion of images.
|---c                     
|-----imgproc             Multi-threaded image processing library.
|-----ugip                GUI for generating circles.
|---python 
|-----dat2excel           Abandonded
|-----makecsv             Generates csv files from imgproc output.
|-------Test              Tests for makecsv.
|-----pandas              Data proc. software depending on Pandas library.
|-----plots               pyplot plot generating software.
|-----remove              Removes some data points from gravity data.
|-----report_plots
|---vs                    Visual studio projects for imgproc and ugip.
|-----imgproc 
|-----ugip
```

## Capture software dataflow pipeline 
The dataflow of the capture software is shown in the following diagram.
![capture dataflow pipeline](https://raw.githubusercontent.com/jimpelton/bsumg13/master/doc/capture.jpg)

## Imgproc software dataflow pipeline
The dataflow of the imgproc post processing software is shown in the following diagram.
![imgproc dataflow pipeline](https://raw.githubusercontent.com/jimpelton/bsumg13/master/doc/average.png)



