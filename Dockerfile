FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar sln y csproj primero (capa cacheada si no cambian dependencias)
COPY *.sln ./
COPY src/BotTelegram/*.csproj ./src/BotTelegram/

# Restaurar dependencias (capa cacheada)
WORKDIR /app/src/BotTelegram
RUN dotnet restore --locked-mode 2>/dev/null || dotnet restore

# Copiar código fuente
WORKDIR /app
COPY src/ ./src/

# Compilar y publicar (sin volver a restaurar)
WORKDIR /app/src/BotTelegram
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "BotTelegram.dll"]
