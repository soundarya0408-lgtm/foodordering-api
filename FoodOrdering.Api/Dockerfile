FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy ENTIRE solution at once (no path issues)
COPY .. .

# Restore + Publish from correct path
WORKDIR /app/FoodOrdering.Api
RUN dotnet restore FoodOrdering.Api.csproj
RUN dotnet publish FoodOrdering.Api.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "FoodOrdering.Api.dll"]
