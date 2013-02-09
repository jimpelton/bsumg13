import os
import argparse

from math import sqrt


#######################################################
# getArgs: get CL arguments
#
def getArgs():
    parser = argparse.ArgumentParser()
    parser.add_argument("-d485", "--Directory485", help="485 Images Directory" )
    parser.add_argument("-d405", "--Directory405", help="405 Images Directory" )
    parser.add_argument("-rf", "--RatiosFile", help="Ratios File")
    parser.add_argument("-wv485", "--WellValues485", help="485 Well Values File")
    parser.add_argument("-wv405", "--WellValues405", help="405 Well Values File")
    parser.add_argument("-gd", "--DirectoryGrav", help="Gravity files directory")

    args = parser.parse_args()

    print( "405 Images Directory {} \n 485 Images Directory {} \n Ratios File {} \n"
           "405 Well Values File {} \n 485 Well Values File {} \n Gravity Directory {} \n".format(
        args.Directory405, args.Directory485, args.RatiosFile,
        args.WellValues405, args.WellValues485, args.DirectoryGrav )
    )

    return args


#######################################################
# read405Files: read in 405 text files
#
def read405Files(files405_list):
    print ('Reading 405 files...'),
    values405_list=[]
    count=0
    for f in files405_list:
        thisfile=open(basedir405+f)
        lines=thisfile.readlines()
        thisfile.close()
        values405_list.append([])
        for s in lines:
            strs=s.split(':')
            #idx=int(strs[0].strip())
            val=int(strs[1].strip())
            values405_list[count].append(val)
        count += 1
    print('{}'.format(count))
    return values405_list

#######################################################
# read485Files: read in 485 text files
#
def read485Files(files485_list):
    print ('Reading 485 files...'),
    values485_list=[]
    count=0
    for f in files485_list:
        thisfile=open(basedir485+f)
        lines=thisfile.readlines()
        thisfile.close()
        values485_list.append([])
        for s in lines:
            strs=s.split(':')
            #idx=int(strs[0].strip())
            val=int(strs[1].strip())
            values485_list[count].append(val)
        count += 1
    print('{}'.format(count))
    return values485_list

#######################################################
#  readGravityFiles: read in gravity text files
#
#  returns: a list of gravity vectory magnitudes for every
#  sample in the filesGravity_list
#
def readGravityFiles(filesGravity_list):
    print ('Reading and calculating gravity vectors...'),
    gravity_list=[]
    for f in filesGravity_list:
        thisfile=open(gravitydir+f)
        line=thisfile.readlines()[0]
        thisfile.close()
        xyzt=[float(i) for i in line.split(' ')]
        gVec = sqrt(xyzt[0]*xyzt[0] + xyzt[1]*xyzt[1] + xyzt[2]*xyzt[2])
        gravity_list.append(gVec)
    print('{}'.format(len(gravity_list)))
    return gravity_list

#################################################################################
#  calculateRatios: compute the ratios of 405/485
#
#  returns: a list of "columns" where each column is actually just another list
#  Each column represents the values at time t for that well value.
#
def calculateRatios(smaller, values405_list, values485_list):
    ratios_list=[]
    for sampNum in range(smaller):
        samples405=values405_list[sampNum]
        samples485=values485_list[sampNum]
        if not (len(samples405)==96 and len(samples485)==96) :
            print('dadnamit, not both have 96 wells en im!')
            exit()
        ratios_list.append([])
        for wellNum in range(96):
            r = samples405[wellNum]/samples485[wellNum]
            ratios_list[sampNum].append( r )
    return ratios_list

#######################################################
#  write405Values
#
def write405Values(values405_filename, values405_list, gravity_list):
    print('Writing 405 values to {0}'.format(values405_filename))
    values405_file=open(values405_filename,'w')
    for i in range(len(values405_list)):
        row=values405_list[i]
        values405_file.write( "{0} ".format(str(i)) )
        values405_file.write( "{0} ".format(str(gravity_list[i])))
        values405_file.write(' '.join(str(x) for x in values405_list[i]))
        values405_file.write('\n')
    values405_file.close()

#######################################################
#  write485Values
#
def write485Values(values485_filename, values485_list, gravity_list):
    print('Writing 485 values to {0}'.format(values485_filename))
    values485_file=open(values485_filename,'w')
    for i in range(len(values485_list)):
        values485_file.write( "{0} ".format(str(i)) )
        values485_file.write( "{0} ".format(str(gravity_list[i])))
        values485_file.write(' '.join(str(x) for x in values485_list[i]))
        values485_file.write('\n')
    values485_file.close()

#######################################################
#  writeRatioValues
#
def writeRatioValues(ratios_filename, ratios_list, gravity_list, ratioAvgs_list):
    print('Writing ratio values to {0}'.format(ratios_filename))
    ratios_file=open(ratios_filename,'w')
    for i in range(len(ratios_list)):
        ratios_file.write( "{0} ".format(str(i)) )
        if gravity_list != None:
            ratios_file.write( "{0} ".format(str(gravity_list[i])))
        ratios_file.write(' '.join(str(x) for x in ratios_list[i]))
        ratios_file.write('\n')
    ratios_file.write(' '.join(str(x) for x in ratioAvgs_list))
    ratios_file.close()

#######################################################
#  writeRatioValues
#  returns: a list of the averages of every well over
#  all the samples in ratios_list
def ratioAveragesPerWell(ratios_list):
    ratioAvgs_list = []
    for wellIdx in range(len(ratios_list)):
        wellSum = 0
        for smpl in range(len(ratios_list[wellIdx])):
            wellSum = wellSum + ratios_list[wellIdx][smpl]

        wellSum = wellSum / len(ratios_list[wellIdx])
        ratioAvgs_list.append(wellSum)
    return ratioAvgs_list

if __name__ == '__main__' :
    args = getArgs()
    basedir485=args.Directory485
    basedir405=args.Directory405
    gravitydir=args.DirectoryGrav
    values485_filename=args.WellValues485
    values405_filename=args.WellValues405
    ratios_filename=args.RatiosFile

    filesGravity_list = [f for f in os.listdir(gravitydir)]
    files485_list = [f for f in os.listdir(basedir485)]
    files405_list = [f for f in os.listdir(basedir405)]

    gravity_list = readGravityFiles(filesGravity_list)
    values405_list = read405Files(files405_list)
    values485_list = read485Files(files485_list)

    if len(files485_list) < len(files405_list):
        smaller=len(files485_list)
        if len(filesGravity_list) < smaller :
            smaller=len(filesGravity_list)
    else:
        smaller=len(files405_list)
        if len(filesGravity_list) < smaller :
            smaller=len(filesGravity_list)

    ratios_list = calculateRatios(smaller, values405_list, values485_list)
    ratioAvgs_list = ratioAveragesPerWell(ratios_list)


    write405Values(values405_filename, values405_list, gravity_list)
    write485Values(values485_filename, values485_list, gravity_list)
    writeRatioValues(ratios_filename, ratios_list, gravity_list, ratioAvgs_list)


