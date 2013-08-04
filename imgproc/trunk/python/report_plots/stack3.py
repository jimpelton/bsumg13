import pandas as pd
import setup
import sys

argv = sys.argv
try:
    start = int(argv[1])
    end = int(argv[2])
except:
    print('Usage: python plot1.py start end')
    sys.exit(0)

types = { 
        'ax0_405' : 'COCO w/ teri',
        'ax0_485' : 'COCO w/ teri',
        'ax1_405' : 'COCO w/o Teri',
        'ax1_485' : 'COCO w/o Teri'
        }

ops   = {  
        'ax0_405' : [setup.moving_average],
        'ax0_485' : [setup.moving_average],
        'ax1_405' : [setup.moving_average],
        'ax1_485' : [setup.moving_average]
        }

wells = {  
        'ax0_405' : ['RH3','RG3','RH2','RG2'],
        'ax0_485' : ['RH3','RG3','RH2','RG2'],
        'ax1_405' : ['RF9','RE9','RF8','RE8'],
        'ax1_485' : ['RF9','RE9','RF8','RE8']
        }

# load the DataFrames with June 5 data.
setup.load_all_df("/home/jim/z/microgravity/2013/ug_data/test_06_05_0811/")

# break out all coco+teri wells.
df405_coco_w_teri = setup.df405[types['ax0_405']]
df485_coco_w_teri = setup.df485[types['ax0_485']]
df405_coco_wo_teri = setup.df405[types['ax1_405']]
df485_coco_wo_teri = setup.df485[types['ax1_485']]

# fetch 4 wells we are interested in.
ax0_wells_405 = df405_coco_w_teri.ix[:, wells['ax0_405']].values
ax0_wells_485 = df485_coco_w_teri.ix[:, wells['ax0_485']].values

# fetch 4 wells we are interested in.
ax1_wells_405 = df405_coco_wo_teri.ix[:, wells['ax1_405']].values
ax1_wells_485 = df485_coco_wo_teri.ix[:, wells['ax1_485']].values

# calc 405/485 ratios.
nprat_coco_w_teri_wells = ax0_wells_405 / ax0_wells_485
nprat_coco_wo_teri_wells = ax1_wells_405 / ax1_wells_485

# moving average of ratios.

nprat_coco_w_teri_wells_ma = setup.moving_average(nprat_coco_w_teri_wells,n=10,axis=0)
nprat_coco_wo_teri_wells_ma = setup.moving_average(nprat_coco_wo_teri_wells,n=10,axis=0)

# get gravitron
npgrav = setup.dfgrav_trunc['mag'].values

# gen plots
import matplotlib.gridspec as gridspec
from pylab import *

gs = gridspec.GridSpec(3,1,width_ratios=[1],height_ratios=[4,4,1])
f = figure()

f.subplots_adjust(hspace=0.0)

ax_ma = [f.add_subplot(gs[i]) for i in range(3)]


# set spacing for y-axis major ticks.
ax_ma[0].yaxis.set_major_locator(MaxNLocator(prune='both', nbins=16))
ax_ma[1].yaxis.set_major_locator(MaxNLocator(prune='both', nbins=16))
ax_ma[2].yaxis.set_major_locator(MaxNLocator(prune='both', nbins=5))


# set spacing of X-axis major ticks
for a in ax_ma:
    a.xaxis.set_major_locator(MaxNLocator(prune='lower', nbins=16))
    a.grid()    

# tick labels off for upper two plots.
for i in range(0,2):
    a = ax_ma[i] 
    a.xaxis.set_major_locator(MaxNLocator(prune=None, nbins=16))
    for l in a.xaxis.get_majorticklabels():
        l.set_visible(False)
ax_ma[2].xaxis.set_major_locator(MaxNLocator(prune=None, nbins=16))

# plot into each individual axis
ax_ma[0].plot(np.arange(start, end, 1), 
              nprat_coco_w_teri_wells_ma[start:end,:])
ax_ma[1].plot(np.arange(start, end, 1), 
              nprat_coco_wo_teri_wells_ma[start:end,:])
ax_ma[2].plot(np.arange(start, end, 1), 
              npgrav[start:end])

# set X-axis "Flight Time" label for gravity axis.
ax_ma[2].set_xlabel("Flight Time", size=14)

# Axis titles.
ax_ma[0].set_title('With Teriparatide')
ax_ma[1].set_title('With Out Teriparatide')
ax_ma[2].set_title('Gravity')

# set vertical label for Y-axis
ax_ma[0].set_ylabel("Ratio w/ Teri", size=14)
ax_ma[1].set_ylabel("Ratio w/o Teri", size=14)
ax_ma[2].set_ylabel("Accel", size=14)

# Legends for each ax_mais
ax_ma[0].legend(ax_ma[0].get_lines(), 
        ('RH3','RG3','RH2','RG2'), 
        loc='upper right', title='Well Number')
ax_ma[1].legend(ax_ma[1].get_lines(), 
        ('F9','E9','F8','E8'),
        loc='upper right', title='Well Number')
ax_ma[2].legend(ax_ma[2].get_lines(),
        ('Accel',), loc='upper right')



show()

