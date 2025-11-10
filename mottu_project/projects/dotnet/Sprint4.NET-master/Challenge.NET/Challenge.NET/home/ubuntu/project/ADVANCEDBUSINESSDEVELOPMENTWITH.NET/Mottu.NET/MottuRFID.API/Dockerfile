# Dockerfile para a API .NET (Corrigido V9 - Foco no Caminho do Projeto)

# Fase de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar todos os arquivos de projeto (.csproj) e o arquivo de solução (.sln)
# O contexto de build é a pasta Mottu.NET (assumindo a correção no docker-compose.yml)
COPY . .

# Restaurar dependências
# O arquivo de projeto da API está em MottuRFID.API/MottuRFID.API.csproj
RUN dotnet restore MottuRFID.API/MottuRFID.API.csproj --no-cache

# Publicar a aplicação API
RUN dotnet publish MottuRFID.API/MottuRFID.API.csproj -c Release -o /app/publish --no-restore

# Fase Final (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Porta padrão do ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "MottuRFID.API.dll"]
