#!/bin/sh

for D in *; do
    if [ -d "${D}" ]; then
        fossil test-delta-create ${D}/origin ${D}/target ${D}/delta
    fi
done
