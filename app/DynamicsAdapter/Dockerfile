FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

ARG PROJECT_NAME
ARG NEXUS_REPO_URL
ARG NEXUS_REPO_USER
ARG NEXUS_REPO_PASSWORD

WORKDIR /src
COPY . .
RUN dotnet restore "DynamicsAdapter.Web/DynamicsAdapter.Web.csproj"
WORKDIR "/src/DynamicsAdapter.Web"
RUN dotnet build "DynamicsAdapter.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DynamicsAdapter.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DynamicsAdapter.Web.dll"]