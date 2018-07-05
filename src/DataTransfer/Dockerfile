FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Monitor/Monitor.csproj Monitor/
RUN dotnet restore Monitor/Monitor.csproj
COPY . .
WORKDIR /src/Monitor
RUN dotnet build Monitor.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Monitor.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Monitor.dll"]

