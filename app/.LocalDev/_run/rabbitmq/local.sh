#!/bin/bash
BASEDIR=$(dirname "$0")

if [ -z "$1" ]; then
  echo ""
  echo "No Config File specified  "
  echo "Usage: $0 <config file>  "
  echo ""
  exit 1
fi

pushd .
cd $BASEDIR

# Create internal network if not already exists
podman network create --ignore fams3

podman run -d --rm --replace --name rabbitmq \
  -p 15672:15672 -p 5672:5672  --net fams3 \
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
podman exec -it rabbitmq rabbitmq-plugins enable rabbitmq_shovel rabbitmq_shovel_management

popd
