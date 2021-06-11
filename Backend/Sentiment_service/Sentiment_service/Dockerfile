#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Sentiment_service/Sentiment_service.csproj", "Sentiment_service/"]
RUN dotnet restore "Sentiment_service/Sentiment_service.csproj"
COPY . .
WORKDIR "/src/Sentiment_service"
RUN dotnet build "Sentiment_service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sentiment_service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sentiment_service.dll"]