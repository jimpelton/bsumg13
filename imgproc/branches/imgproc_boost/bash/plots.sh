#!/bin/bash

TYPE="NONE"

function usage () {
    echo "plots.sh <data-file> <out-dir> <out-prefix> [<begin index> <end index>]"
}


function culture_type () {
    local idx=$1
    case $idx in 
    [0-3]|1[2-5]|2[4-7]|3[6-9]|4[8-9]|5[0-1]) TYPE="Co-Culture";;
    [4-7]|1[6-9]|2[8-9]|3[0-1]|4[0-3]|5[2-5]) TYPE="MC-3T3";;
    [8-9]|1[0-1]|2[0-3]|3[2-5]|4[4-7]|5[6-9]) TYPE="MLO-Y4";;
    6[0-3]) TYPE="Co-Culture Iono";;
    6[4-7]) TYPE="MC-3T3 Iono";;
    6[8-9]|7[0-1]) TYPE="MLO-Y4 Iono";;
    7[2-5]) TYPE="Co-Culture EGTA";;
    7[6-9]) TYPE="MC-3T3 EGTA";;
    8[0-3]) TYPE="MLO-Y4 EGTA";;
    8[4-5]) TYPE="Empty";;
    8[6-7]) TYPE="Agarose";;
    8[8-9]|9[0-1]) TYPE="Cyto w/o Dye";;
    9[2-5]) TYPE="Cyto w/ Dye";;
    esac
}

declare DATAFILE1
declare OUTDIR2
declare PREFIX3
declare LOWER
declare UPPER

if [[ $# -lt 3 ]]; then
    usage
    exit 0
fi

DATAFILE=$1
OUTDIR=$2
PREFIX=$3

if [[ $# -eq 5 ]]; then
    LOWER=$4
    UPPER=$5
else
    LOWER=0
    UPPER=$(awk '{print NF}' $DATAFILE | sort -nu | tail -n 1)
fi

if [[ $LOWER -gt $UPPER ]]; then
    echo "Lower greater than upper."
    usage
    exit 0
fi

echo "Using:"
echo -e "\t Data:      $DATAFILE"
echo -e "\t Outdir:    $OUTDIR"
echo -e "\t Prefix:    $PREFIX"
echo -e "\t Lower idx: $LOWER"
echo -e "\t Upper idx: $UPPER"
echo "Is this information correct? [y/N]"
read yesno

if [[ $yesno == "n" || $yesno == "N" ]]; then
    exit 0
fi

#if [ -z $DATAFILE ]; then
#    echo "$DATAFILE not found."
#    exit 0
#fi
#
#if [ -z $OUTDIR ]; then
#    echo "Make directory $OUTDIR? [y/N]"
#    read yesno
#    if [ yesno == "n" || yesno == "N" ]; then
#        exit 0
#    else 
#        mkdir $OUTDIR
#    fi
#fi

let l=LOWER
let u=UPPER

for j in $(seq $l $u); do
    let col=j+2
    culture_type $j
    #TYPE="Co-Culture"
    echo -ne "\r\033[K"
    echo -ne "Processing $TYPE $j..."
    gnuplot << EOF
set terminal png size 1920,1080
set output "$OUTDIR/$PREFIX$j.png"
plot '$DATAFILE'   using $col with lines axis x1y1 title "$TYPE $j", \
    'grav_5626-7526.dat' using 2 with lines axis x1y2 title "Accel"
EOF

done

exit 0




