import pandas as pd
import pylab as pl
import numpy as np


df405_filepath = '/home/jim/z/microgravity/2013/ug_data/2013_06_04_0840/Data405.dat'
df485_filepath = '/home/jim/z/microgravity/2013/ug_data/2013_06_04_0840/Data485.dat'
dfgrav_filepath = '/home/jim/z/microgravity/2013/ug_data/2013_06_04_0840/grav.dat'
dfphid_filepath = '/home/jim/z/microgravity/2013/ug_data/2013_06_04_0840/phid.dat'


dfgrav_colnames = ["Elapsed", "mag"]
dfphid_colnames = ["Elapsed", "temp_hblk", "temp_ambi", 
                   "uv_hblk", "uv_es1", "uv_es2", "pdiff", 
                   "N/A7", "N/A8", "N/A9", "N/A10"]

def elaptime_from_hmsf(time_str):
    try:
        et = pd.datetime.strptime(time_str, "%H:%M:%S.%f")
    except ValueError:
        et = pd.datetime.strptime(time_str, "%H:%M:%S")
    dt = et - pd.datetime.strptime("0:0:0.000000", "%H:%M:%S.%f")
    return dt

def elaptime_from_seconds(time_str):
    try:
        a = str(time_str).split('.')
        seconds = int(a[0])
        micro = int(a[1])*1000
    except Error as e:
        raise e
    
    minutes, seconds = divmod(seconds, 60)
    hours, minutes = divmod(minutes, 60)

    from pandas import datetime
    from datetime import datetime, timedelta
    return timedelta(hours=hours, minutes=minutes, seconds=seconds, microseconds=micro)

#    string = '{0}:{1}:{2}.{3}'.format(hours,minutes,seconds,millis)
#     
#    et = pd.datetime.strptime(time_str, "%S.%f")
#        et = pd.datetime.strptime("0", "%S")
#        print("couldn't do that time! {} Returning time 0".format(time_str))
#
#    return et - pd.datetime.strptime("0", "%S")

def groupPlates(string):
    if string.startswith("L"):
        return "Left"
    elif string.startswith("R"):
        return "Right"
    else:
        return ""


def load_df(path_str, 
            sep=' ', 
            index_col=0,
            parse_dates=True,
            converters=None):

    if not converters: converters = {'Elapsed': elaptime_from_hmsf}
    return pd.read_csv(path_str,
                       sep=sep,
                       index_col=index_col,
                       parse_dates=parse_dates,
                       converters=converters)

def load_df_raw(path_str, 
                sep=' ', 
                index_col=0, 
                parse_dates=True,
                converters=None,
                colnames=None,
                wellindex=None):

    if not converters: converters = {'Elapsed' : elaptime_from_seconds}

    if not colnames:
        cols, types = makeWellColumnTitles()
        colnames = ['Elapsed']
        for c in cols:
            colnames.append(c)
        idxNames = ['Well Index', 'Well Type'] 
        index = pd.Index(np.array(colnames, types))

    return pd.read_csv(path_str,
                       sep=sep,
                       index_col=index_col,
                       parse_dates=parse_dates,
                       converters=converters,
                       header=None,
                       )
                       

def find_closest_times(search, times):
    """
    Returns times from search_frame that are closest to the times in
    times_frame.
    :param search: The frame or series to search
    :param times:  The frame or series with the times to search for.
    :return: a series with closest timestamps.
    """
    return pd.Series([search.index.asof(dt) for dt in times.index])

def makeWellColumnTitles():
    wellColumnNames = {
        "A1":"Empty",
        "A2":"MC3T3 w/ teri",
        "A3":"MC3T3 w/ teri",
        "A4":"MC3T3 w/o teri",
        "A5":"MC3T3 w/o teri",
        "A6":"MC3T3 w/ teri",
        "A7":"MC3T3 w/ teri",
        "A8":"MLOY4 w/ teri",
        "A9":"MLOY4 w/ teri",
        "A10":"MLOY4 w/o Teri",
        "A11":"MLOY4 w/o Teri",
        "A12":"MC3T3 Iono",
        "B1":"S W/O CELLS W/ DYE",
        "B2":"MC3T3 w/ teri",
        "B3":"MC3T3 w/ teri",
        "B4":"MC3T3 w/o teri",
        "B5":"MC3T3 w/o teri",
        "B6":"MC3T3 w/ teri",
        "B7":"MC3T3 w/ teri",
        "B8":"MLOY4 w/ teri",
        "B9":"MLOY4 w/ teri",
        "B10":"MLOY4 w/o Teri",
        "B11":"MLOY4 w/o Teri",
        "B12":"MC3T3 Iono",
        "C1":"S W/O CELLS W/ DYE",
        "C2":"MC3T3 w/o teri",
        "C3":"MC3T3 w/o teri",
        "C4":"COCO w/o Teri",
        "C5":"COCO w/o Teri",
        "C6":"MLOY4 w/ teri",
        "C7":"MLOY4 w/ teri",
        "C8":"MC3T3 w/o teri",
        "C9":"MC3T3 w/o teri",
        "C10":"COCO w/o Teri",
        "C11":"COCO w/o Teri",
        "C12":"Co-Co EGTA",
        "D1":"S W/O CELLS W/ DYE",
        "D2":"MC3T3 w/o teri",
        "D3":"MC3T3 w/o teri",
        "D4":"COCO w/o Teri",
        "D5":"COCO w/o Teri",
        "D6":"MLOY4 w/ teri",
        "D7":"MLOY4 w/ teri",
        "D8":"MC3T3 w/o teri",
        "D9":"MC3T3 w/o teri",
        "D10":"COCO w/o Teri",
        "D11":"COCO w/o Teri",
        "D12":"Co-Co EGTA",
        "E1":"S W/O CELLS W/O DYE",
        "E2":"MLOY4 w/o Teri",
        "E3":"MLOY4 w/o Teri",
        "E4":"MLOY4 w/o Teri",
        "E5":"MLOY4 w/o Teri",
        "E6":"COCO w/ teri",
        "E7":"COCO w/ teri",
        "E8":"COCO w/o Teri",
        "E9":"COCO w/o Teri",
        "E10":"MC3T3 EGTA",
        "E11":"MC3T3 EGTA",
        "E12":"Co-Co Iono",
        "F1":"S W/O CELLS W/O DYE",
        "F2":"MLOY4 w/o Teri",
        "F3":"MLOY4 w/o Teri",
        "F4":"MLOY4 w/o Teri",
        "F5":"MLOY4 w/o Teri",
        "F6":"COCO w/ teri",
        "F7":"COCO w/ teri",
        "F8":"COCO w/o Teri",
        "F9":"COCO w/o Teri",
        "F10":"MLOY4 Iono",
        "F11":"MLOY4 Iono",
        "F12":"Co-Co Iono",
        "G1":"S W/O CELLS W/O DYE",
        "G2":"COCO w/ teri",
        "G3":"COCO w/ teri",
        "G4":"MLOY4 w/ teri",
        "G5":"MLOY4 w/ teri",
        "G6":"MLOY4 w/ teri",
        "G7":"MLOY4 w/ teri",
        "G8":"COCO w/ teri",
        "G9":"COCO w/ teri",
        "G10":"Cytopore w/ cells w/o dye",
        "G11":"Cytopore w/ cells w/o dye",
        "G12":"MLOY4 EGTA",
        "H1":"AG W/O DYE",
        "H2":"COCO w/ teri",
        "H3":"COCO w/ teri",
        "H4":"MLOY4 w/ teri",
        "H5":"MLOY4 w/ teri",
        "H6":"MC3T3 w/ teri",
        "H7":"MC3T3 w/ teri",
        "H8":"COCO w/ teri",
        "H9":"COCO w/ teri",
        "H10":"Cytopore w/ cells w/o dye",
        "H11":"Ag w/ dye",
        "H12":"MLOY4 EGTA"}

    alpha = [chr(c) for c in range(65, 73)]  # 'A' through 'H'
    cols = []
    types = []
    lr = ["L", "R"]
    for i in range(1, 13):
        for w in range(0, 2):
            p = lr[w]
            for c in alpha:
                ci = str(c)+str(i)
                cols.append(p + ci)
                types.append(wellColumnNames[ci])

    return cols,types

def makeDateTimeIndex(timeList, name="Time"):
    dti = pandas.DatetimeIndex(data=timeList, name=name, tz="UTC")
    dti.name = name
    return dti

def makeElapsedColumn(valuesDict: dict, name="Elapsed"):
    dtMin = self._updateStartTime(valuesDict)
    for frame in valuesDict.values():
        if frame is None:
            continue
        frame.insert(loc=0, column=name,
                value=frame.index.map(lambda x: x-dtMin))

def findStartTime(valuesDict: dict):
    from pytz import timezone as tz
    from pandas import datetime

    dtMin = datetime(2020, 1, 1, tzinfo=tz("UTC"))
    for frame in valuesDict.values():
        if frame is None:
            continue
        m = frame.index.min()
        if m < dtMin:
            dtMin = m
    return dtMin


