# FAMS3 Redis
Redis server for local environments, NOT for Prod

#### Run `local.sh` to create & start container

#### Access the Redis CLI inside the container
```bash
 podman exec -it redis redis-cli
  keys *
  set mykey "Test Value"
  get mykey
  ^D # exit
```

#### Test connection to redis from within the same "fams3" network
```bash
podman run -it --rm --net fams3 busybox:latest telnet redis 6379
  keys *
  del mykey
  keys *
  ^D  # exit
```

# 🔄 Alternative Local Debug Setup
If you plan to locally debug an adapter or component using the dotnet command, make sure to expose the container ports to the host.
You can also start a Redis container locally using Podman (or docker if that’s your setup).

#🧩 Steps
1 - Run Redis locally
Execute the following command in your WSL terminal:
```shell
podman run --rm --replace -d \
  --name redis \
  -p 6379:6379 \
  redis:alpine
```
💡 Tip: You can replace podman with docker if that’s your preferred container runtime.

2 - Verify Redis is running
Execute this command to ensure container is running
```shell
podman ps

Expected output:
CONTAINER ID  IMAGE                            COMMAND       CREATED         STATUS         PORTS                    NAMES
52d0896a98be  docker.io/library/redis:alpine   redis-server  3 second ago    Up 3 second    0.0.0.0:6379->6379/tcp   redis
```
You can verify that Redis is up and responding by running the Redis CLI inside the container:
```bash
podman exec -it redis redis-cli ping

Expected output:
PONG
```
