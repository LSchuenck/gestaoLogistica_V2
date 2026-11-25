# Etapa 1: compila a API (imagem grande, com o SDK do .NET)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS compilacao
WORKDIR /codigo
COPY . .
RUN dotnet publish src/GestaoLogistica.API/GestaoLogistica.API.csproj -c Release -o /publicado

# Etapa 2: executa a API (imagem menor, só com o necessário para rodar)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=compilacao /publicado .
EXPOSE 8080
ENTRYPOINT ["dotnet", "GestaoLogistica.API.dll"]
