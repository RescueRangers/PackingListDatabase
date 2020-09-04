#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["PacklistsWebUI/PacklistsWebUI.csproj", "PacklistsWebUI/"]
COPY ["Packilists.DataAccess/Packilists.Shared.csproj", "Packilists.DataAccess/"]
RUN dotnet restore "PacklistsWebUI/PacklistsWebUI.csproj"
COPY . .
WORKDIR "/src/PacklistsWebUI"
RUN dotnet build "PacklistsWebUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PacklistsWebUI.csproj" -c Release -o /app/publish

FROM base AS final
RUN apt-get update; apt-get install locales -y
RUN locale-gen en_US.UTF-8
ENV LANG='pl_PL.UTF-8' LANGUAGE='pl_PL:en' LC_ALL='pl_PL.UTF-8'
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PacklistsWebUI.dll"]