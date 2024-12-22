# FAMS3 RabbitMQ
RabbitMQ server for local environments, NOT for Prod

- Run `local.sh` to create & start container
- Test connection to rabbitmq from within the same "fams3" network
- `podman run -it --rm --net fams3 busybox:latest telnet rabbitmq 5672`

(note: local.sh  calls `rabbitmqctl import_definitions rabbitmq.json`)