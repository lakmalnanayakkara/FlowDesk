FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FlowDesk.API/FlowDesk.API.csproj", "FlowDesk.API/"]
COPY ["FlowDesk.Core/FlowDesk.Core.csproj", "FlowDesk.Core/"]
COPY ["FlowDesk.Infrastructure/FlowDesk.Infrastructure.csproj", "FlowDesk.Infrastructure/"]

RUN dotnet restore "FlowDesk.API/FlowDesk.API.csproj"

COPY . .
WORKDIR "/src/FlowDesk.API"
RUN dotnet build "FlowDesk.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlowDesk.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlowDesk.API.dll"]