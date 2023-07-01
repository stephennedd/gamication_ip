#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GamificationAPI/GamificationAPI.csproj", "GamificationAPI/"]
RUN dotnet restore "GamificationAPI/GamificationAPI.csproj"

# Install EF Core CLI
RUN dotnet tool install --global dotnet-ef --version 7.0.5
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY . .
WORKDIR "/src/GamificationAPI"
RUN dotnet build "GamificationAPI.csproj" -c Release -o /app/build
RUN dotnet ef migrations add SecondMigration -c ApplicationDbContext -o Data/Migrations
RUN dotnet ef database update
RUN dotnet run seeddata

FROM build AS publish
RUN dotnet publish "GamificationAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GamificationAPI.dll"]