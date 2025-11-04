#!/bin/sh

echo "[Init] Installing curl..."
apk add --no-cache curl

echo "[Init] Enabling RabbitMQ plugins..."
rabbitmq-plugins enable rabbitmq_shovel rabbitmq_shovel_management

echo "[Init] Starting RabbitMQ..."
rabbitmq-server -detached

echo "[Init] Entering wait loop for RabbitMQ management API..."
retry=1
max_retries=10
while [ $retry -lt $max_retries ]; do
  echo "[Loop $retry] Attempting curl to RabbitMQ Management API..."
  response=$(curl -s -u guest:guest http://127.0.0.1:15672/api/overview)
  echo "[Loop $retry] Raw response: $response"

  echo "$response" | grep -q "cluster_name"
  if [ $? -eq 0 ]; then
    echo "[Loop $retry] RabbitMQ is up!"
    break
  fi

  echo "[Loop $retry] Waiting for RabbitMQ to start..."
  retry=$((retry + 1))
  sleep 3
done

if [ $retry -eq $max_retries ]; then
  echo "[Error] RabbitMQ Management API failed to respond after $max_retries retries."
  exit 1
fi

echo "[Init] Stopping detached RabbitMQ..."
rabbitmqctl stop

echo "[Init] Restarting in foreground..."
rabbitmq-server
