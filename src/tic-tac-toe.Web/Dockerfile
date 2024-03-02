FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/tic-tac-toe.Web/tic-tac-toe.Web.csproj", "src/tic-tac-toe.Web/"]
RUN dotnet restore "src/tic-tac-toe.Web/tic-tac-toe.Web.csproj"
COPY . .
WORKDIR "/src/src/tic-tac-toe.Web"
RUN dotnet build "tic-tac-toe.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "tic-tac-toe.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "tic-tac-toe.Web.dll"]
