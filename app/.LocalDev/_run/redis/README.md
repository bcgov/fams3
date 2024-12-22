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