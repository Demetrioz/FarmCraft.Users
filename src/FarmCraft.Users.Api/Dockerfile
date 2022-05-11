#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FarmCraft.Users.Api/FarmCraft.Users.Api.csproj", "FarmCraft.Users.Api/"]
RUN dotnet restore "FarmCraft.Users.Api/FarmCraft.Users.Api.csproj"
COPY . .
WORKDIR "/src/FarmCraft.Users.Api"
RUN dotnet build "FarmCraft.Users.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FarmCraft.Users.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FarmCraft.Users.Api.dll"]