#!/bin/bash
BASEDIR=$(dirname "$0")

pushd .
cd $BASEDIR

# Create internal network if not already exists
podman network create --ignore fams3

podman run --replace --name redis -d \
  -p 6379:6379 --net fams3 \
  redis:alpine

popd
