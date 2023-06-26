# API for GamificationToIP

tech stack: .NET 6.0, C#, PostgreSQL, Entity Framework Core, Docker
![Generic badge](https://img.shields.io/badge/.NET-6.0-green.svg)
![Generic badge](https://img.shields.io/badge/C%23-9.0-blue.svg)
![Generic badge](https://img.shields.io/badge/PostgreSQL-13.4-blue.svg)
![Generic badge](https://img.shields.io/badge/Entity%20Framework%20Core-6.0.0-blue.svg)
![Generic badge](https://img.shields.io/badge/Docker-20.10.8-blue.svg)

## How to run backend?

1. Download Microsoft Visual Studio 2022

2. Open up project which has location of GamificationToIpBackend/GamificationToIP.sl

3. Go to GamificationToIpBackend/appsettings.json and paste your password to postgres db

4. run the project using the following command in terminal:

```sh
dotnet run
```

## How to create a db migration which later be used to create/drop the tables

1. Open up Package Manager Console at the bottom of your Microsoft Visual Studio 2022

2. Run the following command in the Package Manager console:

```sh
Add-Migration SecondMigration -c ApplicationDbContext -o Data/Migrations
```

## How to create tables in db?

1. Create db migration described in previous section

2. Open up Package Manager Console at the bottom of your Microsoft Visual Studio 2022

3. Run the following command in the Package Manager console:

```sh
Update-Database
```

## How to drop tables in db?

1. Create db migration described in previous section

2. Open up Package Manager Console at the bottom of your Microsoft Visual Studio 2022

3. Run the following command in the Package Manager console:

```sh
Drop-Database
```

## How to seed db with the data?

1. Go to /GamificationToIpBackend/GamificationToIP in your terminal

2. Run the following command in the terminal:

```sh
dotnet run seeddata
```
