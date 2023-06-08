FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Microservices/YouTubeService/YouTubeService.csproj", "Microservices/YouTubeService/"]
RUN dotnet restore "Microservices/YouTubeService/YouTubeService.csproj"
COPY . .
WORKDIR "/src/Microservices/YouTubeService"
RUN dotnet build "YouTubeService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YouTubeService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YouTubeService.dll"]
