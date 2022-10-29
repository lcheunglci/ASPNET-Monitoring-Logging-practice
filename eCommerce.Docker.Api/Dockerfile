#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
ENV SimpleProperty="hello-from-BASE-dockerfile"

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["eCommerce.Docker.Api/eCommerce.Docker.Api.csproj", "eCommerce.Docker.Api/"]
RUN dotnet restore "eCommerce.Docker.Api/eCommerce.Docker.Api.csproj"
COPY . .
WORKDIR "/src/eCommerce.Docker.Api"
RUN dotnet build "eCommerce.Docker.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "eCommerce.Docker.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV SimpleProperty="hello-from-FINAL-dockerfile"
ENTRYPOINT ["dotnet", "eCommerce.Docker.Api.dll"]