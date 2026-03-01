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

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
EXPOSE 8080
ENTRYPOINT ["dotnet", "FoodOrdering.Api.dll"]
