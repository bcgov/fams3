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

If the WebHooks section is configured, SearchApi automatically posts a new notification into the webhook collection. The WebHook configuraton in SearchApi is

To enable Person Search notification, configure the SearchApi WebHooks settings.

```json
"SearchApi": {
    "WebHooks": [
      {
        "Name": "dynadapter",
        "Uri":  "http://localhost:5000",
      }
    ] 
  }
```

Search Api Post events to the following routes schema host/PersonSearch/{event}/{searchRequestId}.

| --- | --- | --- |
| Event | URL | Description |
| Completed | host/PersonSearch/Completed/{searchRequestId} | Occurs when an adapter has completed a search. the payload might contains additional information ont the person |
| Accepted | host/PersonSearch/Accepted/{searchRequestId} | Occurs when an adapter has accepted a search |
| Rejected | host/PersonSearch/Rejected/{searchRequestId} | Occurs when an adapter has rejected a search |
| Failed | host/PersonSearch/Failed/{searchRequestId} | Occurs when an adapter has failed on executing a search |

With this configuration the searchApi will post Event to `http://localhost:5000/PersonSearch/{EventName}/{id}` where {id} is a global unique identifier for the search request . the content of the payload is  dependent on the event.

### Search Adapters Events


#### Person Search Completed

When the Adapter finds a match for a particular person, it raises an event that post search completed to dynadapter with the result message.

```json
{
	"person": {
		"firstName": "My name is first",
		"lastName": "My name is last",
		"dateOfBirth": "0001-01-01T00:00:00"
	},
	"personIds": [{
		"kind": "DriversLicense",
		"issuer": "BC Province",
		"number": "123123123"
	}, {
		"kind": "DriversLicense",
		"issuer": "AB Province",
		"number": "123123123"
	}],
	"searchRequestId": "00000000-0000-0000-0000-000000000000",
	"timeStamp": "0001-01-01T00:00:00",
	"providerProfile": {
		"name": "ICBC"
	}
}
```
DynAdapter endpoint will be dynamically generated based on event and posted to the ``PersonSearchController`` at the URI --> /PersonSerach/Completed/{searchRequestId}

#### Person Search Accepted

When a person Search is accepted by data provider, meaning it has sufficient information to conduct a search. An event is raised and posted to dynadapter.

```json
{
	
	"searchRequestId": "00000000-0000-0000-0000-000000000000",
	"timeStamp": "0001-01-01T00:00:00",
	"providerProfile": {
		"name": "ICBC"
	}
}
```
DynAdapter endpoint will be dynamically generated based on event and posted to the ``PersonSearchController`` at the URI --> /PersonSerach/Accepted/{searchRequestId}

#### Person Search Rejected

When a person Search does not meet the minimal requirement for the adapter to conduct a search.

#### Person Search Failed

When the adpater throws an unknown exception.



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

