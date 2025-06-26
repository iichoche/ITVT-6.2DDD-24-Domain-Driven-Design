FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# This COPY line refers to ./BFFApi/BFFApi.csproj (relative to the context above)
COPY ["BFFApi.csproj", "./"]
RUN dotnet restore "BFFApi.csproj"

COPY . .
RUN dotnet publish "BFFApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet","BFFApi.dll"]
