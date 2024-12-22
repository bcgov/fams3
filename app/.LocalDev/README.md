# Local Development Script

This script compiles and runs the FAMS3 applicatons locallty in .NET 3.1 containers in DEBUG mode.

#### Copy `.env.example` to .`env` & Fill in the required properties

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
