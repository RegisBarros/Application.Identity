FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/Application.Identity.Api/Application.Identity.Api.csproj", "src/Application.Identity.Api/"]
RUN dotnet restore "src/Application.Identity.Api/Application.Identity.Api.csproj"
COPY . .
WORKDIR "/src/src/Application.Identity.Api"
RUN dotnet build "Application.Identity.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Application.Identity.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Application.Identity.Api.dll"]
