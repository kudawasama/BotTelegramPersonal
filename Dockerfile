FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar sln y csproj
COPY *.sln ./
COPY src/BotTelegram/*.csproj ./src/BotTelegram/

# Restaurar dependencias
WORKDIR /app/src/BotTelegram
RUN dotnet restore

# Copiar código fuente
WORKDIR /app
COPY src/ ./src/

# Compilar
WORKDIR /app/src/BotTelegram
RUN dotnet publish -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Render expone PORT env var
ENV ASPNETCORE_URLS=http://+:${PORT:-5001}

ENTRYPOINT ["dotnet", "BotTelegram.dll"]
