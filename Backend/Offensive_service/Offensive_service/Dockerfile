#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 5672

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Offensive_service/Offensive_service.csproj", "Offensive_service/"]
RUN dotnet restore "Offensive_service/Offensive_service.csproj"
COPY . .
WORKDIR "/src/Offensive_service"
RUN dotnet build "Offensive_service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Offensive_service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Offensive_service.dll"]