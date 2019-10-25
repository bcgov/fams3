FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["DynamicsAdapter.Web/DynamicsAdapter.Web.csproj", "DynamicsAdapter.Web/"]
RUN dotnet restore "DynamicsAdapter.Web/DynamicsAdapter.Web.csproj"
COPY . .
WORKDIR "/src/DynamicsAdapter.Web"
RUN dotnet build "DynamicsAdapter.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DynamicsAdapter.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DynamicsAdapter.Web.dll"]