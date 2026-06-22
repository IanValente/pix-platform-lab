# ESTÁGIO 1: Construção (A Fábrica Pesada da Microsoft)
# Usamos a imagem completa do SDK (Software Development Kit)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Primeiro, copiamos apenas o arquivo de configuração e restauramos (baixamos) os pacotes NuGet.
# Isso garante que o Docker faça cache das suas bibliotecas e não baixe tudo de novo a cada build.
COPY *.csproj ./
RUN dotnet restore

# Depois copiamos todo o resto do código fonte
COPY . ./

# Executamos o build final focado em performance (Release)
RUN dotnet publish -c Release -o out

# ESTÁGIO 2: Execução (A Loja Leve)
# Usamos a imagem enxuta que contém apenas o motor de execução (Runtime) do ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copiamos apenas a pasta 'out' gerada no Estágio 1
COPY --from=build /app/out .

# Avisamos qual porta o Kestrel (Servidor Web do C#) vai escutar dentro do container
EXPOSE 8080

# Ligamos a ignição apontando para a DLL principal gerada pelo compilador
ENTRYPOINT ["dotnet", "SettlementService.dll"]