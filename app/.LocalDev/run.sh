#!/bin/bash

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
CACHE=${DIR}/.nuget
mkdir -p ${CACHE}

APPDIR=${DIR}/..
DOTENV=${DIR}/.env
NUGET=${DIR}/NuGet.config
IMAGE=mcr.microsoft.com/dotnet/core/sdk:3.1-buster

podman image exists ${IMAGE}
if [ $? != 0 ] ; then
  podman pull ${IMAGE}
fi

pushd . > /dev/null 2>&1
cd ${APPDIR}

echo ""
echo "Compiling ${PROJECT}" ...
echo ""
podman run -it --rm --replace --name sdk3.1 \
  -v ${NUGET}:/app/NuGet.config \
  -v ${CACHE}:/root/.nuget \
  -v ${PWD}:/app/src \
  -w /app \
  --env-file=${DOTENV} \
  ${IMAGE} bash -c "cd src/${PROJECT} && dotnet build -c Debug"

# Create internal network if not already exists
podman network create --ignore fams3 > /dev/null 2>&1

echo ""
echo "Running ${PROJECT}" ...
echo ""
podman run -it --rm --replace --name sdk3.1 \
  -v ${CACHE}:/root/.nuget \
  -v ${PWD}:/app/src \
  -w /app \
  -p 5000:5000 \
  --net fams3 \
  --env-file=${DOTENV} \
  ${IMAGE} bash -c "cd src/${PROJECT} && dotnet run --no-build -c Debug --urls http://0.0.0.0:5000"

popd > /dev/null 2>&1
