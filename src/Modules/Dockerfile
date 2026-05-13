# ================================
# Stage 1: Build
# ================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto para restaurar dependencias
COPY IUSHPAY.Domain/IUSHPAY.Domain.csproj             IUSHPAY.Domain/
COPY IUSHPAY.Application/IUSHPAY.Application.csproj   IUSHPAY.Application/
COPY IUSHPAY.Infrastructure/IUSHPAY.Infrastructure.csproj IUSHPAY.Infrastructure/
COPY IUSHPAY.WebAPI/IUSHPAY.WebAPI.csproj             IUSHPAY.WebAPI/

# Restaurar dependencias (cacheado si los .csproj no cambian)
RUN dotnet restore IUSHPAY.WebAPI/IUSHPAY.WebAPI.csproj

# Copiar el resto del código fuente
COPY IUSHPAY.Domain/           IUSHPAY.Domain/
COPY IUSHPAY.Application/      IUSHPAY.Application/
COPY IUSHPAY.Infrastructure/   IUSHPAY.Infrastructure/
COPY IUSHPAY.WebAPI/           IUSHPAY.WebAPI/

# Publicar en modo Release
RUN dotnet publish IUSHPAY.WebAPI/IUSHPAY.WebAPI.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ================================
# Stage 2: Runtime
# ================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Crear usuario no-root por seguridad
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

# Copiar publicación desde el stage de build
COPY --from=build /app/publish .

# Cambiar propietario de los archivos al usuario no-root
RUN chown -R appuser:appgroup /app

#USER appuser

# Puerto por defecto de ASP.NET Core
EXPOSE 8080

# Variables de entorno base
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "IUSHPAY.WebAPI.dll"]
