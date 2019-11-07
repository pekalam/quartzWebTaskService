FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["QuartzWebTaskService.HTTP", "QuartzWebTaskService.HTTP/"]
COPY ["WebApi", "WebApi/"]
RUN dotnet restore "QuartzWebTaskService.HTTP/QuartzWebTaskService.HTTP.csproj"
RUN dotnet restore "WebApi/WebApi.csproj"
RUN dotnet build "WebApi/WebApi.csproj" -c Release -o /app
FROM build AS publish
RUN dotnet publish "WebApi/WebApi.csproj" -c Release -o /app
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebApi.dll"]