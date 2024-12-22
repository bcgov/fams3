#!/bin/bash
BASEDIR=$(dirname "$0")

if [ -z "$1" ]; then
  echo "Usage: $0 <Config File>"
  echo "example: $0 rabbitmq.json"
  exit 1
fi

pushd .
cd $BASEDIR

podman run -d --replace --name rabbitmq \
  -p 15672:15672 -p 5672:5672 \
  docker.io/rabbitmq:management-alpine

# Wait until RabbitMQ is up
status=""
while [[ -z "$status" ]]; do
  status=$(curl -s -u guest:guest http://localhost:15672/api/overview | grep "cluster_name")

  if [[ -z "$status" ]]; then
    echo "Waiting for RabbitMQ to start..."
    sleep 3
  fi
done

podman cp $1 rabbitmq:/tmp

podman exec -it rabbitmq rabbitmqctl import_definitions /tmp/rabbitmq.json

popd
