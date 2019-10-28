# fams3

FAMS-search-api is a dotnet core rest service to execute people search accross multiple data providers.

## Project Structure

    .
    ├── app                     # Application Source Files.
    ├── .gitignore              # Git ignore.
    └── README.md               # This file.

## SearchApi

### Configuration

#### RabbitMq

Configure RabbiMq using the following ENV variables:

| Property | Required | Description |
| --- | --- | --- |
| RABBITMQ__HOST | no | RabbitMq Host |
| RABBITMQ__PORT | no | RabbitMq Port |
| RABBITMQ__USERNAME | no | RabbitMq UserName |
| RABBITMQ__PASSWORD | no | RabbitMq Password |

*Notes*

> the variables key have 2 underscores

#### OpenApi

The Search Api uses [NSwag](https://github.com/RicoSuter/NSwag) to autogenerate api specification from the code.
To turn on the swagger Ui, set `ASPNETCORE_ENVIRONMENT=Development` environment variable, this should not be use in `production`.

#### Tracing

The Search Api uses [opentracing](https://opentracing.io/) instrumentation for distributed tracing.

It uses [Jaeger](https://www.jaegertracing.io/) implementation to monitor and troubleshoot transactions and reference the [jeager-client-csharp](https://github.com/jaegertracing/jaeger-client-csharp).

The csharp client is set up to use [configuration via environment](https://github.com/jaegertracing/jaeger-client-csharp#configuration-via-environment).

*Notes*

> Set `JAEGER_SERVICE_NAME` if you want the tracer to ship tracing logs.  
> Set `JAEGER_SAMPLER_TYPE=const` if you want to sample all your traces.

## Run on Docker

download and install [Docker](https://www.docker.com)

Run

```shell
cd app/SearchApi
docker-compose up
```

Application health can be checked [here](http://localhost:5050/health).
SwaggerUi can be accessed [here](http://localhost:5050/swagger).
OpenApi specification can be accessed [here](http://localhost:5050/swagger/v1/swagger.json).

## Run

download and install [dotnet core 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0)

Optionaly you can configure `jaeger` client to send traces:

![asp-config](docs/aspnet.configuration.env.png)

Run

```shell
cd app/SearchApi/SearchApi.Web
dotnet run
```

Application health can be checked [here](http://localhost:5000/health).
