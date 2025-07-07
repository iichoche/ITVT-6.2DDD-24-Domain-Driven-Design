# 1) Build-stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 2) Kopieer nuget.config en projectfile
COPY nuget.config .
COPY ["BFFApi.csproj", "./"]

# 3) Restore met expliciete configfile
RUN dotnet restore "BFFApi.csproj" --configfile nuget.config

# 4) Kopieer de rest en publish
COPY . .
RUN dotnet publish "BFFApi.csproj" -c Release -o /app/publish \
    --no-restore

# 5) Runtime-stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "BFFApi.dll"]
