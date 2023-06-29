#!/bin/bash

set -e

# Wait for the PostgreSQL database to be ready
until pg_isready --timeout=0 --dbname=test --host=app_db --port=5432 >/dev/null 2>&1; do
  sleep 1
done

# Run the EF Core commands
dotnet ef database drop --context ApplicationDbContext --force
dotnet ef migrations add SecondMigration --context ApplicationDbContext --output Data/Migrations
dotnet ef database update --context ApplicationDbContext

# Start the application
dotnet GamificationAPI.dll
