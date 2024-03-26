#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Bumblebee/Bumblebee.csproj", "Bumblebee/"]
COPY ["ActivityWrapper/ActivityWrapper.csproj", "ActivityWrapper/"]
COPY ["Isango.Entities/Isango.Entities.csproj", "Isango.Entities/"]
COPY ["Isango.Service.Contract/Isango.Service.Contract.csproj", "Isango.Service.Contract/"]
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
COPY ["ApplicationCacheManager/ApplicationCacheManager.csproj", "ApplicationCacheManager/"]
COPY ["CacheManager/CacheManager.csproj", "CacheManager/"]
COPY ["DiscountRuleEngine/DiscountRuleEngine.csproj", "DiscountRuleEngine/"]
COPY ["Isango.Register/Isango.Register.csproj", "Isango.Register/"]
RUN dotnet restore "Bumblebee/Bumblebee.csproj"
COPY . .
WORKDIR "/src/Bumblebee"
RUN dotnet build "Bumblebee.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bumblebee.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bumblebee.dll"]