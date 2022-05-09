#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FarmCraft.Users.Core/FarmCraft.Users.Core.csproj", "FarmCraft.Users.Core/"]
RUN dotnet restore "FarmCraft.Users.Core/FarmCraft.Users.Core.csproj"
COPY . .
WORKDIR "/src/FarmCraft.Users.Core"
RUN dotnet build "FarmCraft.Users.Core.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FarmCraft.Users.Core.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FarmCraft.Users.Core.dll"]