# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
# ENV ASPNETCORE_URLS=http://+:8080

# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia TODO el repo (incluye .sln y cualquier Directory.*.props)
COPY . .

# Restaura usando la solución (ajusta el nombre exacto de tu .sln)
RUN dotnet restore "AsisyaApi.sln"

# Publica la API
WORKDIR /src/src/Asisya.Api
RUN dotnet publish "Asisya.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Asisya.Api.dll"]
