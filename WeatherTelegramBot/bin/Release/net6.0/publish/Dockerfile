FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY . .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet WeatherTelegramBot.dll