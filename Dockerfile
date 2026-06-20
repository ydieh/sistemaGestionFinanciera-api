# --- Etapa 1: build ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos solo los .csproj primero para aprovechar el cache de capas de Docker
COPY src/Core/BancoSol.Domain/BancoSol.Domain.csproj src/Core/BancoSol.Domain/
COPY src/Core/BancoSol.Application/BancoSol.Application.csproj src/Core/BancoSol.Application/
COPY src/Infrastructure/BancoSol.Infrastructure/BancoSol.Infrastructure.csproj src/Infrastructure/BancoSol.Infrastructure/
COPY src/Presentation/BancoSol.API/BancoSol.API.csproj src/Presentation/BancoSol.API/
RUN dotnet restore src/Presentation/BancoSol.API/BancoSol.API.csproj

# Ahora copiamos el resto del código fuente y publicamos
COPY src/ src/
RUN dotnet publish src/Presentation/BancoSol.API/BancoSol.API.csproj -c Release -o /app/publish --no-restore

# --- Etapa 2: runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Railway inyecta el puerto en la variable PORT; ASPNETCORE_URLS se fija en tiempo de ejecución
# mediante la variable de entorno configurada en el servicio (ver README, sección Despliegue).
EXPOSE 8080

ENTRYPOINT ["dotnet", "BancoSol.API.dll"]
