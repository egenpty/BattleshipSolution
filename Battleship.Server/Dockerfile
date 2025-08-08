# Set default .NET version to 7.0 (override with build args as needed)
ARG DOTNET_VERSION=7.0

# Stage 1: Build the app
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
ARG DOTNET_VERSION
WORKDIR /app

# Copy csproj and restore as separate layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and publish
COPY . ./
RUN dotnet publish -f net${DOTNET_VERSION} -c Release -o /app/out

# Stage 2: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime
WORKDIR /app

COPY --from=build /app/out ./

EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000
ENV DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "Battleship.Server.dll"]
