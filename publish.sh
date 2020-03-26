﻿#!/bin/bash

pushd $1

rm *.tgz*

FILE_NAME=`npm pack`
curl -F package=@${FILE_NAME} https://${PUBLISH_KEY}@${HOST_NAME}/${PROJECT_NAME}/

popd
