# fams3

FAMS-search-api is a dotnet core rest service to execute people search accross multiple data providers.

## Project Structure

    .
    ├── app                     # Application Source Files.
    ├── .gitignore              # Git ignore.
    └── README.md               # This file.

## SearchApi

### Tracing

The Search Api uses [opentracing](https://opentracing.io/) instrumentation for distributed tracing.

It uses [Jaeger](https://www.jaegertracing.io/) implementation to monitor and troubleshoot transactions and reference the [jeager-client-csharp](https://github.com/jaegertracing/jaeger-client-csharp).

The csharp client is set up to use [configuration via environment](https://github.com/jaegertracing/jaeger-client-csharp#configuration-via-environment).

*Notes*

> You **must** set `JAEGER_SERVICE_NAME`.  
> Configure `JAEGER_SAMPLER_TYPE=const` if you want to sample all your traces.

## Run on Docker

download and install [Docker](https://www.docker.com)

Run

```shell
cd app/SearchApi
docker-compose up
```

Application health can be checked [here](http://localhost:5050/health).

## Run

download and install [dotnet core 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0)

Configure the following environment variables:

![asp-config](docs/aspnet.configuration.env.png)

Run

```shell
cd app/SearchApi/SearchApi.Web
dotnet run
```

Application health can be checked [here](http://localhost:5000/health).
