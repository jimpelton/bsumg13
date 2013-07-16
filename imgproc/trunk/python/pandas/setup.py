import pandas as pd
import pylab as pl
import numpy as np


df405_filepath = '/home/jim/z/microgravity/2013/ug_data/2013_06_04_0840/Data405.dat'
df485_filepath = '/home/jim/z/microgravity/2013/ug_data/2013_06_04_0840/Data485.dat'
dfgrav_filepath = '/home/jim/z/microgravity/2013/ug_data/2013_06_04_0840/grav.dat'
dfphid_filepath = '/home/jim/z/microgravity/2013/ug_data/2013_06_04_0840/phid.dat'


dfgrav_colnames = ["Elapsed", "mag"]
dfphid_colnames = ["Elapsed", "temp_hblk", "temp_ambi", "uv_hblk", "uv_es1", "uv_es2", "pdiff", 
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
                colnames=None):

    if not converters: converters = {'Elapsed' : elaptime_from_seconds}
    if not colnames:
        tits = makeWellColumnTitles()
        colsnames = ['Elapsed']
        for title in tits:
            colnames.append(title)

    return pd.read_csv(path_str,
                       sep=sep,
                       index_col=index_col,
                       parse_dates=parse_dates,
                       converters=converters,
                       header=None,
                       names=colnames)
                       

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
    alpha = [chr(c) for c in range(65, 73)]  # 'A' through 'H'
    cols = []
    lr = ["L", "R"]
    for i in range(1, 13):
        for w in range(0, 2):
            p = lr[w]
            for c in alpha:
                cols.append(p + str(c) + str(i))
    return cols

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

# def truncate(data_frame, times, skipcol=1, cols=None):
#     """
#
#     :param data_frame:
#     :param times:
#     :param skipcol:
#     :param cols:
#     :return:
#     """
#     Gavgs = []
#     Tavgs = []
#     for ts in times:
#         idx = data_frame.index.searchsorted(ts)
#         idxLow = idx-3
#         idxHigh = idx+3
#         if idxLow < 0:
#             idxLow = 0
#         if idxHigh > len(data_frame.index):
#             idxHigh = len(data_frame.index)
#
#         Gs = data_frame.ix[idxLow:idxHigh,:]
#         Gavg = Gs.ix[:,skipcol:].mean()
#         Tavgs.append(ts)
#         Gavgs.append([data_frame['Elapsed'].ix[idx], Gavg])
#
#     dti = pd.DatetimeIndex(Tavgs)
#     return pd.DataFrame(data=Gavgs, index=dti, columns=cols)




def test_load():
    df405=load_df_raw('/home/jim/z/microgravity/2013/ug_data/test_06_04_0840/Data405.dat')
