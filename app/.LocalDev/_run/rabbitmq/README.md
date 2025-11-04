# FAMS3 RabbitMQ
RabbitMQ server for local environments, NOT for Prod

- Run `local.sh` to create & start container
- Test connection to rabbitmq from within the same "fams3" network
- `podman run -it --rm --net fams3 busybox:latest telnet rabbitmq 5672`

(note: local.sh  calls `rabbitmqctl import_definitions rabbitmq.json`)

# 🔄 Alternative Local Debug Setup
If you plan to locally debug an adapter or component using the dotnet command, make sure to expose the container ports to the host.
You can also run RabbitMQ locally using Podman (or replace podman with docker if that’s what you have installed).

# 🧩 Steps
1 - Run RabbitMQ locally
Execute the following command in your WSL terminal:
```shell
podman run -d --rm --replace \
  --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  -v /<Path-To-Project>/app/.LocalDev/_run/rabbitmq:/opt/rabbitmq-init:ro \
  -v /<Path-To-Project>/app/.LocalDev/_run/rabbitmq/rabbitmq.conf:/etc/rabbitmq/rabbitmq.conf:ro \
  docker.io/rabbitmq:management-alpine \
  /bin/sh /opt/rabbitmq-init/rabbitmq-startup.sh
```
💡 Tip: You can replace podman with docker if that’s your preferred container runtime.

2 - Verify RabbitMQ is running
Execute this command to ensure container is running
```shell
podman ps

Expected output:
CONTAINER ID  IMAGE                                         COMMAND               CREATED        STATUS        PORTS                                             NAMES
dbf5c921775c  docker.io/library/rabbitmq:management-alpine  /bin/sh /opt/rabb...  4 seconds ago  Up 4 seconds  0.0.0.0:5672->5672/tcp, 0.0.0.0:15672->15672/tcp  rabbitmq
```
Open your browser on the host machine and visit:
👉 http://localhost:15672

If the login page loads, RabbitMQ is up and accessible.
