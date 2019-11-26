# FAMS3

## Project Structure

    .
    ├── app                     # Application Source Files.
    |   ├── SearchApi           # Search Api
    |   └── DynamicAdapter      # Dynamic Adapter
    ├── .gitignore              # Git ignore.
    └── README.md               # This file.
    └── openshift               # Build and Deploy Configurations.
    └── bin                     # Useful scripts.

## RUN

Create a `.env` file from the `.env.template`

Configure the necessary variables in `.env`

```shell
docker-compose up
```

| app | url |
| --- | --- |
| Search API Swagger UI | [http://localhost:5050/swagger](http://localhost:5050/swagger) |
| Search API OpenApi Specification | [http://localhost:5050/swagger/v1/swagger.json](http://localhost:5050/swagger/v1/swagger.json) |
| Search API Health | [http://localhost:5060/health](http://localhost:5060/health) |
| Dynamics Adapter Health | [http://localhost:5060/health](http://localhost:5060/health) |
| ICBC Adatpter Health | [http://localhost:5051/health](http://localhost:5060/health) |
| RabbitMq Management Console | [http://localhost:15672/](http://localhost:15672) |
| Jaeger OpenTrace Monitoring | [http://localhost:16686/search](http://localhost:16686/search) |
| HealthChecks UI | [http://localhost:6060/healthchecks-ui](http://localhost:6060/healthchecks-ui) |

## SearchApi

FAMS-search-api is a dotnet core rest service to execute people search accross multiple data providers.

### Configuration

Configure RabbiMq using the following ENV variables:

| Property | Required | Description |
| --- | --- | --- |
| RABBITMQ__HOST | no | RabbitMq Host |
| RABBITMQ__PORT | no | RabbitMq Port |
| RABBITMQ__USERNAME | no | RabbitMq UserName |
| RABBITMQ__PASSWORD | no | RabbitMq Password |

*Notes*

> the variables key have 2 underscores

### Match Found Notifications

If the WebHooks section is configured, SearchApi automatically posts a new notification into the webhook collection.

To enable MatchFound notification, they have to be configured through the SearchApi WebHooks settings.

```json
"SearchApi": {
    "WebHooks": [
      {
        "Name": "Adapter1",
        "Uri":  "http://localhost:5000" 
      }
    ] 
  }
```

With this configuration the searchApi will post MatchFound to `http://localhost:5000/{id}` where {id} is a global unique identifier. the content of the payload is a MatchFound object

```json
{
    "FistName": "firstName",
    "LastName": "lastName",
    "DateOfBirth": "2001-01-01"
}
```

### OpenApi

The Search Api uses [NSwag](https://github.com/RicoSuter/NSwag) to autogenerate api specification from the code.
To turn on the swagger Ui, set `ASPNETCORE_ENVIRONMENT=Development` environment variable, this should not be use in `production`.

### Tracing

The Search Api uses [opentracing](https://opentracing.io/) instrumentation for distributed tracing.

It uses [Jaeger](https://www.jaegertracing.io/) implementation to monitor and troubleshoot transactions and reference the [jeager-client-csharp](https://github.com/jaegertracing/jaeger-client-csharp).

The csharp client is set up to use [configuration via environment](https://github.com/jaegertracing/jaeger-client-csharp#configuration-via-environment).

*Notes*

> Set `JAEGER_SERVICE_NAME` if you want the tracer to ship tracing logs.  
> Set `JAEGER_SAMPLER_TYPE=const` if you want to sample all your traces.

### Run

download and install [dotnet core 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0)

Optionaly you can configure `jaeger` client to send traces:

![asp-config](docs/aspnet.configuration.env.png)

Run

```shell
cd app/SearchApi/SearchApi.Web
dotnet run
```

Application health can be checked [here](http://localhost:5000/health).

FAMS-search-api is a dotnet core rest service to execute people search accross multiple data providers.

## Search Adapters

The Search Adpaters a worker that execute a search for a specific data providers.

### Configuration

| Property | Required | Description |
| --- | --- | --- |
| RABBITMQ__HOST | no | RabbitMq Host |
| RABBITMQ__PORT | no | RabbitMq Port |
| RABBITMQ__USERNAME | no | RabbitMq UserName |
| RABBITMQ__PASSWORD | no | RabbitMq Password |

### Search Adapters Events

#### MatchFound

When the Adapter found a match for a particular person

#### Person Search Accepted

When a person Search is accepted, meaning it has sufficient information to conduct a search.

#### Person Search Rejected

When a person Search does not meet the minimal requirement for the adapter to conduct a search.

#### Person Search Failed

When the adpater throws an unknown exception.

## DynamicAdapter

FAMS-dynamic-adapter is a dotnet core rest service that polls dynamics to retrieve ready to search request and execute search using the Search API.

### Configuration

| Property | Required | Description |
| --- | --- | --- |
| RABBITMQ__HOST | no | RabbitMq Host |
| RABBITMQ__PORT | no | RabbitMq Port |
| RABBITMQ__USERNAME | no | RabbitMq UserName |
| RABBITMQ__PASSWORD | no | RabbitMq Password |
| SCHEDULER__CRON | no | a dotnet Quartz cron expression |
| SEARCHAPI__BASEURL | yes | the base path of the search api |
| OAUTH__RESOURCEURL | yes | the resource URL you required the token for|
| OAUTH__CLIENTID | yes | the Oauth Client id |
| OAUTH__SECRET | yes | the Oauth secret |
| OAUTH__OAUTHURL | yes | the Oauth URL |
| OAUTH__USERNAME | yes | the Oauth username |
| OAUTH__PASSWORD | yes | the Oauth password |

*Notes*

> the variables key have 2 underscores

### Tracing

The Dynamic Adapter uses [opentracing](https://opentracing.io/) instrumentation for distributed tracing.

It uses [Jaeger](https://www.jaegertracing.io/) implementation to monitor and troubleshoot transactions and reference the [jeager-client-csharp](https://github.com/jaegertracing/jaeger-client-csharp).

The csharp client is set up to use [configuration via environment](https://github.com/jaegertracing/jaeger-client-csharp#configuration-via-environment).

*Notes*

> Set `JAEGER_SERVICE_NAME` if you want the tracer to ship tracing logs.  
> Set `JAEGER_SAMPLER_TYPE=const` if you want to sample all your traces.

### Run

download and install [dotnet core 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0)

Optionaly you can configure `jaeger` client to send traces:

![asp-config](docs/images/aspnet.configuration.env.png)

Run

```shell
cd app/DynamicAdapter/DynamicAdapter.Web
dotnet run
```

## Monitoring

### AspNetCore Diagnostics 

All dotnet core applications support [AspNetCore Diagnostics](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks) and are compatible with [HealthCheck UI](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks#healthcheckui-and-failure-notifications)

Look at the [HealthCheck UI docker image](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/docs/ui-docker.md) for deployment and configure it using environment variables.

You can find a configuration exemple in our [docker-compose.yml](docker-compose.yml) under watchdog.

![healthChecksUi](docs/images/healthcheck-ui.png)

