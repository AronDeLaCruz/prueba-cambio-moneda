# Etapa 1: compilar
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore CambioMoneda.Api/CambioMoneda.Api.csproj
RUN dotnet publish CambioMoneda.Api/CambioMoneda.Api.csproj -c Release -o /app/publish --no-restore

# Etapa 2: ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "CambioMoneda.Api.dll"]