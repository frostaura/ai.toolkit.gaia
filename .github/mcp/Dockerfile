FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/Gaia.Mcp.Server/Gaia.Mcp.Server.csproj src/Gaia.Mcp.Server/
RUN dotnet restore src/Gaia.Mcp.Server/Gaia.Mcp.Server.csproj
COPY src/ src/
RUN dotnet publish src/Gaia.Mcp.Server/Gaia.Mcp.Server.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Gaia.Mcp.Server.dll"]
