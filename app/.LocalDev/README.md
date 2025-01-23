# Local Development Script

This script compiles and runs the FAMS3 applicatons locallty in .NET 3.1 containers in DEBUG mode.

#### Copy `.env.example` to .`env` & Fill in the required properties
To get OAUTH values, view "dynamics-oauth-credentials" secrets in OpenShift e.g. https://console.apps.silver.devops.gov.bc.ca/k8s/ns/dfb30e-dev/secrets/dynamics-oauth-credentials

#### Start redis and rabbitmq containers
- `_run/rabbitmq/local.sh <config file>` # (will need `rabbitmq.json` config file)
- `_run/redis/local.sh`
- Verify rabbitMQ running:  `http://localhost:15672/` (guest/guest)
- See the README files in each `run` directory for more information

#### Execute `run.sh` for the desired app.
- `./run.sh DynamicsAdapter/DynamicsAdapter.Web`
- `./run.sh SearchApi/SearchRequest.Adaptor`
- `./run.sh SearchApi/SearchApi.Web`

#### Verify application is running:
http://localhost:5000/health
http://localhost:5000/swagger

#### Start VPNKit if connected to VPN
This is needed because the GovBC VPN blocks even local network traffic between WSL and Windows.
- `https://github.com/sakai135/wsl-vpnkit/`

Extract vpnkit.tar.gz
`#tar xvzf vpn/vpnkit.tar.gz`

Run wsl-vpnkit
`./start.sh`

## DEBUGGING

There are several possible combinations of ways to run and debug everything locally. It is recommend to use WSL and run the services you are not debugging as docker containers and then use VS IDE/Code using WSL profile for the target service.

For example:
```shell
_run/rabbitmq/local.sh rabbitmq.json
_run/redis/local.sh
./run.sh SearchApi/SearchApi.Web
./run.sh DynamicsAdapter/DynamicsAdapter.Web

