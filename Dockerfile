FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-end
WORKDIR /app
COPY . .
RUN dotnet restore "./ApiGateway/ApiGateway.csproj" 
RUN dotnet publish "./ApiGateway/ApiGateway.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:6.0 

EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=build-end /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]