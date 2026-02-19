# Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files first for caching
COPY DrocsidApi/DrocsidApi.csproj DrocsidApi/
COPY LogicLibrary/LogicLibrary.csproj LogicLibrary/

# Restore
RUN dotnet restore DrocsidApi/DrocsidApi.csproj

# Copy all files
COPY . .

# Build and publish
WORKDIR /src/DrocsidApi
RUN dotnet publish DrocsidApi.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Non-root user
RUN adduser --disabled-password --gecos "" appuser
USER appuser

ENTRYPOINT ["dotnet", "DrocsidApi.dll"]