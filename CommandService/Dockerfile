FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env 
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 8090 
USER app

WORKDIR /app
COPY --from=build-env /app/out .
ENV ASPNETCORE_HTTP_PORTS=8090
ENTRYPOINT ["dotnet", "CommandService.dll"]
