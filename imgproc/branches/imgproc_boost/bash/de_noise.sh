#!/bin/bash

###############################################################################
# Requires imagemagick convert utitility.
# runs convert -noise 3 on the uG raw images from M9 cameras.
#
# Author: Jim Pelton <jimpelton@u.boisestate.edu>
#


function con () {
    local start=$1
    local end=$2
    local indir=$3
    local outdir=$4
    local prefix=$5

    echo 
    for i in $(seq -f "%05g" $start $end); do
        echo -en "$prefix$i.raw...\r"
        convert -noise 3 -size 2592x1944 \
        gray:$indir/$prefix$i.raw \
        gray:$outdir/$prefix$i.raw;
    done
}


function usage () {
    echo -n "Usage: "
    echo "de_noise.sh <start idx> <end idx> <in dir> <out dir> <prefix>"
}

if [ $# -lt 5 ]; then
    usage
    exit 0
fi

LOW=$1
HIGH=$2
INDIR=$3
OUTDIR=$4
PREFIX=$5

echo "Start index: $LOW"
echo "End index: $HIGH"
echo "Input dir: $INDIR"
echo "Output dir: $OUTDIR"
echo "Prefix: $PREFIX"

echo -n "Is this information correct? [y/N] "
read yesno

if [[ "$yesno" = "y" || "$yesno" = "Y" ]]; then
    echo "Begin coversion...this will take HOURS!"
    con $LOW $HIGH $INDIR $OUTDIR $PREFIX
fi

exit 0

