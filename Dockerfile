FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar proyecto y restaurar dependencias
COPY src/BotTelegram/*.csproj ./src/BotTelegram/
RUN dotnet restore src/BotTelegram/BotTelegram.csproj

# Copiar todo y compilar
COPY . .
RUN dotnet publish src/BotTelegram/BotTelegram.csproj -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BotTelegram.dll"]
