FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY DevTrack.sln ./
COPY DevTrack.Api/DevTrack.Api.csproj DevTrack.Api/
COPY DevTrack.Application/DevTrack.Application.csproj DevTrack.Application/
COPY DevTrack.Domain/DevTrack.Domain.csproj DevTrack.Domain/
COPY DevTrack.Infrastructure/DevTrack.Infrastructure.csproj DevTrack.Infrastructure/
COPY DevTrack.Tests/DevTrack.Tests.csproj DevTrack.Tests/
RUN dotnet restore DevTrack.Api/DevTrack.Api.csproj

COPY . .
RUN dotnet publish DevTrack.Api/DevTrack.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "DevTrack.Api.dll"]
