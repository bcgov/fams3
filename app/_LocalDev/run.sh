#!/bin/bash

# endpoints
# http://localhost:5000/health
# http://localhost:5000/api/People​/search

if [ -z "$1" ]; then
  echo "Usage: $0 <Project Path>"
  echo "example: $0 SearchApi/SearchApi.Web"
  exit 1
fi

podman -v > /dev/null 2>&1
if [ $? != 0 ] ; then
  echo ""
  echo "Podman not installed."
  echo "This script requires podman.  Please install and try again."
  exit 1
fi

PROJECT=$1
DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
APPDIR=${DIR}/..
CACHE=${DIR}/.nuget
SRC=${APPDIR}/${PROJECT}
mkdir -p ${CACHE}

DOTENV=${DIR}/.env
NUGET=${APPDIR}/NuGet.config

podman image exists sdk:3.1-buster
if [ $? != 0 ] ; then
  podman pull mcr.microsoft.com/dotnet/core/sdk:3.1-buster
fi

pushd . > /dev/null 2>&1
cd ${APPDIR}

echo ""
echo "Compiling ${PROJECT}" ...
echo ""
podman run -it --rm --name sdk3.1 \
  -v ${NUGET}:/app/NuGet.config \
  -v ${CACHE}:/root/.nuget \
  -v ${PWD}:/app/src \
  -w /app \
  --env-file=${DOTENV} \
  sdk:3.1-buster bash -c "cd src/${PROJECT} && dotnet build -c Debug"

echo ""
echo "Running ${PROJECT}" ...
echo ""
podman run -it --rm --name sdk3.1 \
  -v ${CACHE}:/root/.nuget \
  -v ${PWD}:/app/src \
  -w /app \
  -p 5000:5000 \
  --env-file=${DOTENV} \
  sdk:3.1-buster bash -c "cd src/${PROJECT} && dotnet run --no-build -c Debug --urls http://0.0.0.0:5000"

popd > /dev/null 2>&1
