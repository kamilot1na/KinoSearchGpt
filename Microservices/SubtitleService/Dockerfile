FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Microservices/SubtitleService/SubtitleService.csproj", "Microservices/SubtitleService/"]
RUN dotnet restore "Microservices/SubtitleService/SubtitleService.csproj"
COPY . .
WORKDIR "/src/Microservices/SubtitleService"
RUN dotnet build "SubtitleService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SubtitleService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SubtitleService.dll"]
