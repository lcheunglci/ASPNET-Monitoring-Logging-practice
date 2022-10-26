#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["eCommerce.InvoiceGenerator.Console/eCommerce.InvoiceGenerator.Console.csproj", "eCommerce.InvoiceGenerator.Console/"]
RUN dotnet restore "eCommerce.InvoiceGenerator.Console/eCommerce.InvoiceGenerator.Console.csproj"
COPY . .
WORKDIR "/src/eCommerce.InvoiceGenerator.Console"
RUN dotnet build "eCommerce.InvoiceGenerator.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "eCommerce.InvoiceGenerator.Console.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "eCommerce.InvoiceGenerator.Console.dll"]