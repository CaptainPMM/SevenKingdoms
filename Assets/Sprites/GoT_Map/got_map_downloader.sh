#!/bin/bash

for y in {0..25}
do
    for x in {0..14}
    do
        linkFilename=y
        linkFilename+=$y
        linkFilename+=x
        linkFilename+=$x
        linkFilename+=.png

        targetFilename=y
        targetFilename+=$(printf %04d $y)
        targetFilename+=x
        targetFilename+=$(printf %04d $x)
        targetFilename+=.png

        echo ""
        echo "### Downloading link file $linkFilename with target file $targetFilename..."
        curl -o ./Map_Images/$targetFilename http://viewers-guide.hbo.com/mapimages/8/$linkFilename
    done
done