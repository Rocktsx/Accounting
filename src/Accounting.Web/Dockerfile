# 请参阅 https://aka.ms/customizecontainer 以了解如何自定义调试容器，以及 Visual Studio 如何使用此 Dockerfile 生成映像以更快地进行调试。

# 此阶段用于在快速模式(默认为调试配置)下从 VS 运行时
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# 此阶段用于生成服务项目
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["src/Accounting.Web/Accounting.Web.csproj", "src/Accounting.Web/"]
COPY ["aspire/Accounting.ServiceDefaults/Accounting.ServiceDefaults.csproj", "aspire/Accounting.ServiceDefaults/"]
COPY ["src/Accounting.Application/Accounting.Application.csproj", "src/Accounting.Application/"]
COPY ["src/Accounting.Domain/Accounting.Domain.csproj", "src/Accounting.Domain/"]
COPY ["src/Accounting.Domain.Shared/Accounting.Domain.Shared.csproj", "src/Accounting.Domain.Shared/"]
COPY ["src/Accounting.Application.Contracts/Accounting.Application.Contracts.csproj", "src/Accounting.Application.Contracts/"]
COPY ["src/Accounting.HttpApi/Accounting.HttpApi.csproj", "src/Accounting.HttpApi/"]
COPY ["src/Accounting.EntityFrameworkCore/Accounting.EntityFrameworkCore.csproj", "src/Accounting.EntityFrameworkCore/"]
RUN dotnet restore "./src/Accounting.Web/Accounting.Web.csproj"
COPY . .
WORKDIR "/src/src/Accounting.Web"
RUN dotnet build "./Accounting.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build
WORKDIR /app/build
RUN dotnet dev-certs https -v -ep authserver.pfx -p BB7BBDD7-5D33-48D6-9B6F-C49E3EF468ED

# 此阶段用于发布要复制到最终阶段的服务项目
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Accounting.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 此阶段在生产中使用，或在常规模式下从 VS 运行时使用(在不使用调试配置时为默认值)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish . 
ENTRYPOINT ["dotnet", "Accounting.Web.dll"]