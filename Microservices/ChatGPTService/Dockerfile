FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Microservices/ChatGPTService/ChatGPTService.csproj", "Microservices/ChatGPTService/"]
RUN dotnet restore "Microservices/ChatGPTService/ChatGPTService.csproj"
COPY . .
WORKDIR "/src/Microservices/ChatGPTService"
RUN dotnet build "ChatGPTService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatGPTService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatGPTService.dll"]
