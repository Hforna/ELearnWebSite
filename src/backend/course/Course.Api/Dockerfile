# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/backend/course/Course.Api/Course.Api.csproj", "src/backend/course/Course.Api/"]
COPY ["src/backend/course/shared/Course.Communication/Course.Communication.csproj", "src/backend/course/shared/Course.Communication/"]
COPY ["src/backend/course/Course.Domain/Course.Domain.csproj", "src/backend/course/Course.Domain/"]
COPY ["src/backend/course/Course.Application/Course.Application.csproj", "src/backend/course/Course.Application/"]
COPY ["src/backend/course/shared/Course.Exception/Course.Exception.csproj", "src/backend/course/shared/Course.Exception/"]
RUN dotnet restore "./src/backend/course/Course.Api/Course.Api.csproj"
COPY . .
WORKDIR "/src/src/backend/course/Course.Api"
RUN dotnet build "./Course.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

RUN apt-get update && apt-get install -y ffmpeg

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Course.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Course.Api.dll"]