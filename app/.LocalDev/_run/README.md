# RUN Locally (WSL2)

## Intro
We will run the previously the compiled project inside a dotnet 3.1 runtime container. This is how the app will run on openshift.  Refer to the parent README for basic system requirements

## Setup Local Development Environment

### 1.  Confirm you can login to the Nexus repo URL with your browser
https://nexus-https-dfb30e-tools.apps.silver.devops.gov.bc.ca/

### 2.  In  Terminal, navigate to `src` and open in code editor
```bash
cd src
code .
```
You may need to install the WSL Extention for VSCode if not aleady installed.  Do this or things will be very slow!  You should see "WSL" in the bottom left corner of VSCode.  If not, start again.


## Setup Runtime Environment

### 1. Start RabbitMQ Container
Start a local RabbitMQ Container using `_run/rabbitmq/local.sh ` (use the provided config file) <br>
Verify you can access the RabbitMQ Queues dashboard: http://localhost:15672/#/queues (guest/guest) <br>
Verify connection to RabbitMQ from another container (see `rabbitmq/README.md`)

### 2. Start a Redis  Container
Start a local Redis Container using `_run/redis/local.sh` <br>
Verify connection to Redis from another container (see `redis/README.md`)

### 3. Copy env.example to .env and edit as needed for the module you are running

## Run Project in Container
#### 1. Pull the required ASP.NET image
`podman pull mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim`

#### 2. Execute run.sh for the module`
example:
- `./run.sh DynamicsAdapter/DynamicsAdapter.Web`
- `./run.sh SearchApi/SearchRequest.Adaptor`
- `./run.sh SearchApi/SearchApi.Web`

Verify app running: http://localhost:5000/health

Note: The first SDK build will take some time to create its NuGet cache, etc. The cache is kept in a local `.nuget` directory so subsequent runs will build more quickly.
