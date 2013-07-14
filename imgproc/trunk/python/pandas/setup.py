import pandas as pd
import pylab as pl
import numpy as np


df405_filepath = '/home/jim/z/microgravity/2013/ug_data/2013_06_04_0840/Data405.dat'
df485_filepath = '/home/jim/z/microgravity/2013/ug_data/2013_06_04_0840/Data485.dat'
dfgrav_filepath = '/home/jim/z/microgravity/2013/ug_data/2013_06_04_0840/grav.dat'
dfphid_filepath = '/home/jim/z/microgravity/2013/ug_data/2013_06_04_0840/phid.dat'

def elaptime(time_str):
    try:
        et = pd.datetime.strptime(time_str, "%H:%M:%S.%f")
    except ValueError:
        et = pd.datetime.strptime(time_str, "%H:%M:%S")
    dt = et - pd.datetime.strptime("0:0:0.000000", "%H:%M:%S.%f")
    return dt


def groupPlates(string):
    if string.startswith("L"):
        return "Left"
    elif string.startswith("R"):
        return "Right"
    else:
        return ""


def load_df(path_str, sep=' ', index_col=0, parse_dates=True, converters=None):

    if not converters: converters = {'Elapsed': elaptime}
    return pd.read_csv(path_str,
                       sep=' ',
                       index_col=index_col,
                       parse_dates=parse_dates,
                       converters=converters)

def truncate(data_frame, times, skipcol=1, cols=None):
    """

    :param data_frame:
    :param times:
    :param skipcol:
    :param cols:
    :return:
    """
    Gavgs = []
    Tavgs = []
    for ts in times:
        idx = data_frame.index.searchsorted(ts)
        idxLow = idx-3
        idxHigh = idx+3
        if idxLow < 0:
            idxLow = 0
        if idxHigh > len(data_frame.index):
            idxHigh = len(data_frame.index)

        Gs = data_frame.ix[idxLow:idxHigh,:]
        Gavg = Gs.ix[:,skipcol:].mean()
        Tavgs.append(ts)
        Gavgs.append([data_frame['Elapsed'].ix[idx], Gavg])

    dti = pd.DatetimeIndex(Tavgs)
    return pd.DataFrame(data=Gavgs, index=dti, columns=cols)

def find_closest_times(search, times):
    """
    Returns times from search_frame that are closest to the times in
    times_frame.
    :param search: The frame or series to search
    :param times:  The frame or series with the times to search for.
    :return: a series with closest timestamps.
    """
    return pd.Series([search.index.asof(dt) for dt in times.index])

