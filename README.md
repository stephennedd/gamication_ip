# gamication_ip

## How to run backend?

1. Download Microsoft Visual Studio 2022

2. Open up project which has location of GamificationToIpBackend/GamificationToIP.sl

3. Go to GamificationToIpBackend/appsettings.json and paste your password to postgres db

4. Click on green triangle on top to run the backend

## How to create a db migration which later be used to create/drop the tables

1. Open up Package Manager Console at the bottom of your Microsoft Visual Studio 2022

2. Run: Add-Migration SecondMigration -c ApplicationDbContext -o Data/Migrations

## How to create tables in db?

1. Create db migration described in previous section

2. Open up Package Manager Console at the bottom of your Microsoft Visual Studio 2022

3. Run: Update-Database

## How to drop tables in db?

1. Create db migration described in previous section

2. Open up Package Manager Console at the bottom of your Microsoft Visual Studio 2022

3. Run: Drop-Database

## How to seed db with the data?

1. Go to /GamificationToIpBackend/GamificationToIP in your terminal 

2. Run: dotnet run seeddata

