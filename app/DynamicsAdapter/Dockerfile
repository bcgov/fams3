FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

# This can cause a "not consumed" warning, but that can be ignored
ARG PROJECT_NAME NEXUS_REPO_URL NEXUS_REPO_USER NEXUS_REPO_PASSWORD

WORKDIR "/src/${PROJECT_NAME}"
COPY . /src
RUN dotnet publish "${PROJECT_NAME}.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app
COPY --from=build /app .
# ENTRYPOINT ["sh","-c","dotnet ${PROJECT_NAME}.dll"]
ENTRYPOINT ["dotnet", "${PROJECT_NAME}.dll"]
