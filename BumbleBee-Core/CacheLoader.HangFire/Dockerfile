FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["CacheLoader.HangFire/CacheLoader.HangFire.csproj", "CacheLoader.HangFire/"]
COPY ["AsyncBooking.HangFire/AsyncBooking.HangFire.csproj", "AsyncBooking.HangFire/"]
COPY ["Isango.Service.Contract/Isango.Service.Contract.csproj", "Isango.Service.Contract/"]
COPY ["Isango.Entities/Isango.Entities.csproj", "Isango.Entities/"]
COPY ["ServiceAdapters/ServiceAdapters.csproj", "ServiceAdapters/"]
COPY ["Factories/Factories.csproj", "Factories/"]
COPY ["Logger.Contract/Logger.Contract.csproj", "Logger.Contract/"]
COPY ["Logger/Logger.csproj", "Logger/"]
COPY ["Util/Util.csproj", "Util/"]
COPY ["Isango.Service/Isango.Service.csproj", "Isango.Service/"]
COPY ["ApplicationCacheManager.Contract/ApplicationCacheManager.Contract.csproj", "ApplicationCacheManager.Contract/"]
COPY ["CacheManager.Contract/CacheManager.Contract.csproj", "CacheManager.Contract/"]
COPY ["Isango.Mailer/Isango.Mailer.csproj", "Isango.Mailer/"]
COPY ["Isango.Persistence.Contract/Isango.Persistence.Contract.csproj", "Isango.Persistence.Contract/"]
COPY ["Isango.Persistence/Isango.Persistence.csproj", "Isango.Persistence/"]
COPY ["PriceRuleEngine/PriceRuleEngine.csproj", "PriceRuleEngine/"]
COPY ["TableStorageOperations/TableStorageOperations.csproj", "TableStorageOperations/"]
COPY ["DataDumping.HangFire/DataDumping.HangFire.csproj", "DataDumping.HangFire/"]
COPY ["Isango.Register/Isango.Register.csproj", "Isango.Register/"]
COPY ["ActivityWrapper/ActivityWrapper.csproj", "ActivityWrapper/"]
COPY ["ApplicationCacheManager/ApplicationCacheManager.csproj", "ApplicationCacheManager/"]
COPY ["CacheManager/CacheManager.csproj", "CacheManager/"]
COPY ["DiscountRuleEngine/DiscountRuleEngine.csproj", "DiscountRuleEngine/"]
COPY ["DataDumpingQueue.Hangfire/DataDumpingQueue.Hangfire.csproj", "DataDumpingQueue.Hangfire/"]

RUN dotnet restore "CacheLoader.HangFire/CacheLoader.HangFire.csproj"
COPY . .
WORKDIR "/src/CacheLoader.HangFire"
RUN dotnet build "CacheLoader.HangFire.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "CacheLoader.HangFire.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CacheLoader.HangFire.dll"]
