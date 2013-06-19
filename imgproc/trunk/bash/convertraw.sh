#!/bin/bash


dir_in=$1
dir_out=$2

files=($dir_in/*.raw)
numfiles=${#files[*]}


echo -e "Found $numfiles files.\n"

for i in $(seq 0 100 $numfiles); 
do
    f=${files[$i]}
    
    outname=$(basename "$f")
    echo -ne "\rProcessing... $outname"
    convert -size "2592x1944" -depth 16 -resize "1920x1080" gray:"$f" "$dir_out/$outname.png"

done



#for f in $dir_in/*.raw;
#do
#    echo "Processing $f"
#    outname=$(basename "$f")
#    convert -size "2592x1944" -depth 16 -resize "1920x1080" gray:"$f" "$dir_out/$outname.png"
#done



