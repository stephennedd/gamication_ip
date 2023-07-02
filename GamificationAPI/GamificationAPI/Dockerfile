#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

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
COPY . .
WORKDIR "/src/GamificationAPI"
RUN dotnet build "GamificationAPI.csproj" -c Release -o /app/build

RUN dotnet dev-certs https -ep /app/build/https/aspnetapp.pfx -p password

FROM build AS publish
RUN dotnet publish "GamificationAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /app/build/https/aspnetapp.pfx /https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="password"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="/https/aspnetapp.pfx"
ENTRYPOINT ["dotnet", "GamificationAPI.dll"]