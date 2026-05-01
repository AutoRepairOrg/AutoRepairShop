# ================================
# Build stage
# ================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AutoRepairShop.Api/AutoRepairShop.Api.csproj", "AutoRepairShop.Api/"]
COPY ["AutoRepairShop.Application/AutoRepairShop.Application.csproj", "AutoRepairShop.Application/"]
COPY ["AutoRepairShop.Domain/AutoRepairShop.Domain.csproj", "AutoRepairShop.Domain/"]
COPY ["AutoRepairShop.Infrastructure/AutoRepairShop.Infrastructure.csproj", "AutoRepairShop.Infrastructure/"]

RUN dotnet restore "AutoRepairShop.Api/AutoRepairShop.Api.csproj"

COPY . .
WORKDIR "/src/AutoRepairShop.Api"
RUN dotnet publish -c Release -o /app/publish

# ================================
# Runtime stage
# ================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

ENTRYPOINT ["dotnet", "AutoRepairShop.Api.dll"]