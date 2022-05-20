# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /src

# Set config environment variables
ENV MAPQUESTAPI_KEY=NULL
ENV DATABASE_USERNAME=admin
ENV DATABASE_PASSWORD=1234
ENV DATABASE_ADDRESS=127.0.0.1
ENV DATABASE_PORT=5432
ENV DATABASE_NAME=tourplanner

# Copy csproj and restore as distinct layers
COPY src/TourPlanner.Server.sln ./

COPY src/Server/TourPlanner.Server.BL.API.Test/ Server/TourPlanner.Server.BL.API.Test/
COPY src/Server/TourPlanner.Server.BL.API/ Server/TourPlanner.Server.BL.API/
COPY src/Server/TourPlanner.Server.BL.Common/ Server/TourPlanner.Server.BL.Common/
COPY src/Server/TourPlanner.Server.BL.MapQuestAPI/ Server/TourPlanner.Server.BL.MapQuestAPI/
COPY src/Server/TourPlanner.Server.DAL/ Server/TourPlanner.Server.DAL/

COPY src/Common/TourPlanner.Common.Models/ Common/TourPlanner.Common.Models/

RUN dotnet restore

COPY . .

# Copy everything else and build
WORKDIR /src/Server/TourPlanner.Server.BL.API
RUN dotnet publish -c Release -o out 

# Delete existing config file (config through env vars)
RUN rm config.json

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY --from=build-env /src/Server/TourPlanner.Server.BL.API/out ./
ENTRYPOINT ["dotnet", "TourPlanner.Server.BL.API.dll"]