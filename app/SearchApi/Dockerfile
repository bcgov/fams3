FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY . .
RUN dotnet restore "SearchApi.Web/SearchApi.Web.csproj"
WORKDIR "/src/SearchApi.Web"
RUN dotnet build "SearchApi.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SearchApi.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SearchApi.Web.dll"]
