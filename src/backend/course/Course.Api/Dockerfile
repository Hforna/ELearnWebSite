# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
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

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Course.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app

USER root
RUN echo "Types: deb" > /etc/apt/sources.list.d/debian.sources && \
    echo "URIs: http://deb.debian.org/debian" >> /etc/apt/sources.list.d/debian.sources && \
    echo "Suites: bookworm bookworm-updates bookworm-security" >> /etc/apt/sources.list.d/debian.sources && \
    echo "Components: main non-free non-free-firmware" >> /etc/apt/sources.list.d/debian.sources && \
    echo "Signed-By: /usr/share/keyrings/debian-archive-keyring.gpg" >> /etc/apt/sources.list.d/debian.sources && \
    echo "deb http://deb.debian.org/debian bookworm main non-free non-free-firmware" > /etc/apt/sources.list.d/non-free.list && \
    echo "deb http://deb.debian.org/debian bookworm-updates main non-free non-free-firmware" >> /etc/apt/sources.list.d/non-free.list && \
    echo "deb http://security.debian.org/debian-security bookworm-security main non-free non-free-firmware" >> /etc/apt/sources.list.d/non-free.list && \
    apt-get update -y && \
    apt-get install -y --no-install-recommends ffmpeg && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*
USER app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Course.Api.dll"]