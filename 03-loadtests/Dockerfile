FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

COPY . .

WORKDIR /app/src/FriendsApi.Host
RUN dotnet restore 
RUN dotnet publish -c Release -o out

FROM base
WORKDIR /app
COPY --from=build /app/src/FriendsApi.Host/out ./

ENTRYPOINT ["dotnet", "FriendsApi.Host.dll"]
