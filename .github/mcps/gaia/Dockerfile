FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/fa.mcp.gaia/ fa.mcp.gaia/
RUN dotnet publish fa.mcp.gaia/fa.mcp.gaia.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/docs

ENV MCP_TRANSPORT=http
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "fa.mcp.gaia.dll"]
