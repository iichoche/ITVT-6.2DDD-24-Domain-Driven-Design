################################################################################
# 1) Build stage
################################################################################
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1.1 Copy only the project file (in root)
COPY ["BCImplementatie.csproj", "./"]

# 1.2 Restore dependencies
RUN dotnet restore "BCImplementatie.csproj"

# 1.3 Copy the rest of the source code
COPY . .

# 1.4 Publish the project
RUN dotnet publish "BCImplementatie.csproj" -c Release -o /app/publish

################################################################################
# 2) Runtime stage
################################################################################
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# 2.1 Copy the published output
COPY --from=build /app/publish .

# 2.2 Expose port 80
EXPOSE 80

# after COPY --from=build /app/publish . 
COPY appsettings.json . 

# 2.3 Start the app (DLL must match the project name)
ENTRYPOINT ["dotnet", "BCImplementatie.dll"]
