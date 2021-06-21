FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
ARG PROJECT_NAME

ARG NEXUS_REPO_URL
ARG NEXUS_REPO_URI
ARG NEXUS_REPO_USER
ARG NEXUS_REPO_PASSWORD

WORKDIR /src
COPY . .
RUN dotnet restore "${PROJECT_NAME}/${PROJECT_NAME}.csproj"

WORKDIR "/src/${PROJECT_NAME}"
RUN dotnet build "${PROJECT_NAME}.csproj" -c Release -o /app

FROM build AS publish
ARG PROJECT_NAME
RUN dotnet publish "${PROJECT_NAME}.csproj" -c Release -o /app

FROM base AS final
ARG PROJECT_NAME
ENV DLL_NAME="${PROJECT_NAME}.dll"
COPY --from=publish /app .

ENTRYPOINT ["sh","-c","dotnet ${DLL_NAME}"]
