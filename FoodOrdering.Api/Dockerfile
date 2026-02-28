FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Clean first (remove old bin/obj)
RUN find . -name "bin" -type d -prune -exec rm -rf {} + && \
    find . -name "obj" -type d -prune -exec rm -rf {} +

# Copy csproj + restore
COPY FoodOrdering.Api/FoodOrdering.Api.csproj FoodOrdering.Api/
COPY FoodOrdering.Application/*.csproj Food
