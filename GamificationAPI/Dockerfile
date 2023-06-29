# Use the appropriate base image that includes the .NET SDK and Entity Framework Core tools
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy and restore dependencies
COPY ["GamificationAPI/GamificationAPI.csproj", "GamificationAPI/"]
RUN dotnet restore "GamificationAPI/GamificationAPI.csproj"

# Copy the application code
COPY . .

WORKDIR "/src/GamificationAPI"

# Build the application
RUN dotnet build "GamificationAPI.csproj" -c Release -o /app/build

# Rest of the Dockerfile remains unchanged
FROM build AS publish
RUN dotnet publish "GamificationAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Add the following lines to the Dockerfile to run the database commands
RUN apt-get update \
    && apt-get install -y postgresql-client \
    && apt-get clean

# Install Entity Framework Core tools
RUN dotnet tool install --global dotnet-ef --version 7.0.0

# Set the PATH environment variable to include the Entity Framework Core tools path
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]



