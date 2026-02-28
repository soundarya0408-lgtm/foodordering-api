FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy csproj files first for layer caching
COPY FoodOrdering.Api/FoodOrdering.Api.csproj FoodOrdering.Api/
COPY FoodOrdering.Application/*.csproj FoodOrdering.Application/
COPY FoodOrdering.Domain/*.csproj FoodOrdering.Domain/
COPY FoodOrdering.Infrastructure/*.csproj FoodOrdering.Infrastructure/
COPY FoodOrdering.sln .

WORKDIR /app/FoodOrdering.Api
RUN dotnet restore

# Copy source
COPY .. .

RUN dotnet publish FoodOrdering.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from
