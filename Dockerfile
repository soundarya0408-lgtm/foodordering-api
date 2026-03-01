# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY FoodOrdering.sln .
COPY FoodOrdering.Api/FoodOrdering.Api.csproj FoodOrdering.Api/
COPY FoodOrdering.Application/FoodOrdering.Application.csproj FoodOrdering.Application/
COPY FoodOrdering.Domain/FoodOrdering.Domain.csproj FoodOrdering.Domain/
COPY FoodOrdering.Infrastructure/FoodOrdering.Infrastructure.csproj FoodOrdering.Infrastructure/

RUN dotnet restore FoodOrdering.Api/FoodOrdering.Api.csproj

COPY . .

RUN dotnet publish FoodOrdering.Api/FoodOrdering.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

RUN mkdir -p /app/data

ENV ASPNETCORE_URLS=http://+:8080
ENV PORT=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "FoodOrdering.Api.dll"]
