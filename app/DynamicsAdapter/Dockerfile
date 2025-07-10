FROM mcr.microsoft.com/dotnet/sdk:8.0.410 AS build

ARG PROJECT_NAME
ARG NEXUS_REPO_URL
ARG NEXUS_REPO_USER
ARG NEXUS_REPO_PASSWORD

WORKDIR "/src/${PROJECT_NAME}"

# Install NSwag globally and ensure PATH is correctly set
RUN dotnet tool install --global NSwag.ConsoleCore --version 14.4.0 > /dev/null 2>&1

# Ensure PATH is available for later build layers
ENV PATH="${PATH}:/root/.dotnet/tools"
# Also expose NSwag path to MSBuild via custom env var
ENV NSwagPath="/root/.dotnet/tools/nswag"

# Copy all source code
COPY . /src

# Move to the project directory and build
WORKDIR "/src/${PROJECT_NAME}"
RUN dotnet build "${PROJECT_NAME}.csproj" -c Release -o /app && rm -rf /tmp/*

FROM build AS publish
ARG PROJECT_NAME
RUN dotnet publish "${PROJECT_NAME}.csproj" -c Release -o /app/publish && rm -rf /tmp/*

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ARG PROJECT_NAME
ENV DLL_NAME="${PROJECT_NAME}.dll"
COPY --from=publish /app/publish .

ENTRYPOINT ["sh","-c","dotnet /app/${DLL_NAME}"]
