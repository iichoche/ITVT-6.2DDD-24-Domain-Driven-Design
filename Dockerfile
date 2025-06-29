# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and restore as distinct layers
COPY ZorgtechnologieProductV1.sln ./
COPY ZorgtechnologieProduct.API/ ./ZorgtechnologieProduct.API/
COPY ZorgtechnologieProduct.Application/ ./ZorgtechnologieProduct.Application/
COPY ZorgtechnologieProduct.Domain/ ./ZorgtechnologieProduct.Domain/
COPY ZorgtechnologieProduct.Infrastructure/ ./ZorgtechnologieProduct.Infrastructure/

WORKDIR /src/ZorgtechnologieProduct.API
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ZorgtechnologieProduct.API.dll"]
