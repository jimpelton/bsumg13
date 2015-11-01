# bsumg13
Automatically exported from code.google.com/p/bsumg13

TL;DR: WARNING: DRAGONS IN THIS!

## Contents
bsumg13 contains the in-flight capture and post-flight data processing
software used by the 2013 BSU Microgravity team. 
This repository that can be found at: 

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
