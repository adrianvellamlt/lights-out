FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore ./src/LightsOutWebApp/LightsOutWebApp.csproj

# Copy everything else and build
RUN dotnet publish -c Release -o out ./src/LightsOutWebApp/LightsOutWebApp.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "LightsOut.Web.dll"]