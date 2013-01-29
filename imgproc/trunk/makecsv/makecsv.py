import os
from math import sqrt

#######################################################
#  input                                              #
#######################################################
basedir485='E://data/microgravity/test_output/14_06_2012_08_41_56_334/Camera485/'
basedir405='E://data/microgravity/test_output/14_06_2012_08_41_56_334/Camera405/'
gravitydir='E://data/microgravity/test_output/datapackets_test/'


#######################################################
#  output                                             #
#######################################################
values485_csv='E://data/microgravity/test_output/wellValues_485.dat'
values405_csv='E://data/microgravity/test_output/wellValues_405.dat'
ratios_csv='E://data/microgravity/test_output/ratios.dat'

files485_list = [f for f in os.listdir(basedir485)]
files405_list = [f for f in os.listdir(basedir405)]
filesGravity_list = [f for f in os.listdir(gravitydir)]

if len(files485_list) < len(files405_list):
    smaller=len(files485_list)
    if len(filesGravity_list) < smaller :
        smaller=len(filesGravity_list)
else:
    smaller=len(files405_list)
    if len(filesGravity_list) < smaller :
        smaller=len(filesGravity_list)

values405_list=[]
values485_list=[]
ratios_list=[]
gravity_list=[]

#######################################################
#  read 405 files									  #
#######################################################
print ('Reading 405 files...'),
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

#######################################################
#  read 485 files									  #
#######################################################
print ('Reading 485 files...'),
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

print ('Reading and calculating gravity vectors...'),
for f in filesGravity_list:
    thisfile=open(gravitydir+f)
    line=thisfile.readlines()[0]
    thisfile.close()
    xyzt=[float(i) for i in line.split(' ')]
    gVec = sqrt(xyzt[0]*xyzt[0] + xyzt[1]*xyzt[1] + xyzt[2]*xyzt[2])
    gravity_list.append(gVec)
print('{}'.format(len(gravity_list)))

#######################################################
#  Calc ratios   									  #
#######################################################
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

#######################################################
#  write 405 values									  #
#######################################################
print('Writing 405 values to {0}'.format(values405_csv))
values405_file=open(values405_csv,'w')
for i in range(len(values405_list)):
    row=values405_list[i]
    values405_file.write( "{0} ".format(str(i)) )
    values405_file.write( "{0} ".format(str(gravity_list[i])))
    values405_file.write(' '.join(str(x) for x in values405_list[i]))
    values405_file.write('\n')
values405_file.close()

#######################################################
#  write 485 values									  #
#######################################################
print('Writing 485 values to {0}'.format(values485_csv))
values485_file=open(values485_csv,'w')
for i in range(len(values485_list)):
    values485_file.write( "{0} ".format(str(i)) )
    values485_file.write( "{0} ".format(str(gravity_list[i])))
    values485_file.write(' '.join(str(x) for x in values485_list[i]))
    values485_file.write('\n')
values485_file.close()

#######################################################
#  write ratio values								  #
#######################################################
print('Writing ratio values to {0}'.format(ratios_csv))
ratios_file=open(ratios_csv,'w')
for i in range(len(ratios_list)):
    ratios_file.write( "{0} ".format(str(i)) )
    ratios_file.write( "{0} ".format(str(gravity_list[i])))
    ratios_file.write(' '.join(str(x) for x in ratios_list[i]))
    ratios_file.write('\n')
ratios_file.close()

