# <img height="55" src="\build\icon.png" align="center"> .NET Database Feature Management


[![Build Status](https://github.com/teociaps/FeatureManagement.Database/actions/workflows/build.yml/badge.svg)](https://github.com/teociaps/FeatureManagement.Database/actions/workflows/build.yml)
[![Test Status](https://github.com/teociaps/FeatureManagement.Database/actions/workflows/test.yml/badge.svg)](https://github.com/teociaps/FeatureManagement.Database/actions/workflows/test.yml)

**.NET Feature Management Database** extends [Feature Management] for retrieving feature definitions from various databases.
It includes abstractions and default implementations to facilitate easy integration with your .NET applications.

## Index

* [Packages](#packages)
* [Getting Started](#getting-started)
    * [Feature Store](#feature-store)
    * [Service Registration](#service-registration)
    * [Configure Cache](#configure-cache)
* [Consumption](#consumption)
    * [ASP.NET Core Integration](#asp.net-core-integration)
* [Built-in Database Providers](#built-in-database-providers)
    * [Entity Framework Core](#entity-framework-core)
    * [Dapper](#dapper)
    * [MongoDB](#mongodb)

## Packages

| Package | NuGet Version |
| ------- | ------------- |
| [FeatureManagement.Database.Abstractions](https://www.nuget.org/packages/FeatureManagement.Database.Abstractions/) | [![NuGet Version](https://img.shields.io/nuget/v/FeatureManagement.Database.svg?style=flat)](https://www.nuget.org/packages/FeatureManagement.Database.Abstractions/)
| [FeatureManagement.Database.EntityFrameworkCore](https://www.nuget.org/packages/FeatureManagement.Database.EntityFrameworkCore/) | [![NuGet Version](https://img.shields.io/nuget/v/FeatureManagement.Database.svg?style=flat)](https://www.nuget.org/packages/FeatureManagement.Database.EntityFrameworkCore/)
| [FeatureManagement.Database.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/FeatureManagement.Database.EntityFrameworkCore.SqlServer/) | [![NuGet Version](https://img.shields.io/nuget/v/FeatureManagement.Database.svg?style=flat)](https://www.nuget.org/packages/FeatureManagement.Database.EntityFrameworkCore.SqlServer/)
| [FeatureManagement.Database.EntityFrameworkCore.PostgreSQL](https://www.nuget.org/packages/FeatureManagement.Database.EntityFrameworkCore.PostgreSQL/) | [![NuGet Version](https://img.shields.io/nuget/v/FeatureManagement.Database.svg?style=flat)](https://www.nuget.org/packages/FeatureManagement.Database.EntityFrameworkCore.PostgreSQL/)
| [FeatureManagement.Database.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/FeatureManagement.Database.EntityFrameworkCore.Sqlite/) | [![NuGet Version](https://img.shields.io/nuget/v/FeatureManagement.Database.svg?style=flat)](https://www.nuget.org/packages/FeatureManagement.Database.EntityFrameworkCore.Sqlite/)
| [FeatureManagement.Database.EntityFrameworkCore.MySql](https://www.nuget.org/packages/FeatureManagement.Database.EntityFrameworkCore.MySql/) | [![NuGet Version](https://img.shields.io/nuget/v/FeatureManagement.Database.svg?style=flat)](https://www.nuget.org/packages/FeatureManagement.Database.EntityFrameworkCore.MySql/)
| [FeatureManagement.Database.Dapper](https://www.nuget.org/packages/FeatureManagement.Database.Dapper/) | [![NuGet Version](https://img.shields.io/nuget/v/FeatureManagement.Database.svg?style=flat)](https://www.nuget.org/packages/FeatureManagement.Database.Dapper/)
| [FeatureManagement.Database.MongoDB](https://www.nuget.org/packages/FeatureManagement.Database.MongoDB/) | [![NuGet Version](https://img.shields.io/nuget/v/FeatureManagement.Database.svg?style=flat)](https://www.nuget.org/packages/FeatureManagement.Database.MongoDB/)

**Package Purposes**

* _FeatureManagement.Database.Abstractions_
	* Standard functionalities for managing feature flags across various databases
* _FeatureManagement.Database.EntityFrameworkCore_
	* Common package for EF Core with base implementations
* _FeatureManagement.Database.EntityFrameworkCore.SqlServer_
	* Integration with SQL Server database using EF Core
* _FeatureManagement.Database.EntityFrameworkCore.PostgreSQL_
	* Integration with PostgreSQL database using EF Core
* _FeatureManagement.Database.EntityFrameworkCore.Sqlite_
	* Integration with Sqlite database using EF Core
* _FeatureManagement.Database.EntityFrameworkCore.MySql_
	* Integration with MySql database using EF Core
* _FeatureManagement.Database.Dapper_
	* Integration with Dapper
* _FeatureManagement.Database.MongoDB_
	* Integration with MongoDB


## Getting Started

.NET Feature Management Database allows you to manage feature definitions stored in a database.
The built-in `DatabaseFeatureDefinitionProvider` retrieves these definitions and converts them into `FeatureDefinition` objects used by the feature management system.
This setup relies on an implementation of the `IFeatureStore` interface to access the database.

### Entities

Two primary entities are pre-configured for database feature management:

- **Feature**: represents a feature with its associated settings. Each feature has a unique name, a requirement type, and a collection of settings that define how the feature is enabled or disabled.

- **FeatureSettings**: contains the settings for a feature and these define the conditions under which a feature is enabled.
The condition parameters are stored in JSON format and based on Feature Management [built-in feature filter][Feature Management built-in filters] or [contextual feature filter][Feature Management contextual filters] configuration, and can include [custom feature filter][Feature Management custom filters] configuration.

#### Example

Suppose you want to define a feature that is enabled for 50% of the users.
Here is an example of how you can define such a feature and its settings:

```csharp
…
var newFeature = new Feature
{
    Id = Guid.NewGuid(),
    Name = "NewFeature",
    RequirementType = RequirementType.Any,
    Settings = new List<FeatureSettings>
    {
        new FeatureSettings
        {
            Id = Guid.NewGuid(),
            FilterType = FeatureFilterType.Percentage,
            Parameters = "{\"Value\":50}"
        }
    }
}
…
```

### Feature Store

The `IFeatureStore` interface is the core abstraction for retrieving feature data from a database.
Implement this interface to connect to your specific database (e.g., SQL Server, MongoDB, etc.).

```csharp
public class MyFeatureStore : IFeatureStore
{
    // Implementation to fetch feature definitions from your database
}
```

See [built-in database implementations](#built-in-database-providers).

### Service Registration

Database feature management relies on .NET Core dependency injection.
You can register the necessary services with a single line of code:
```csharp
services.AddDatabaseFeatureManagement<MyFeatureStore>();
```

> [!NOTE]
> In the context of database solutions, the feature management services will be added as scoped services.

> [!IMPORTANT]
> To use database feature management, you need to register an implementation of **IFeatureStore**.

### Configure Cache

Enhance performance and reduce database load using `WithCacheService`:

- **Default Options**
  
    ```csharp
    services.AddDatabaseFeatureManagement<MyFeatureStore>()
        .WithCacheService();
    ```

    > By default, the inactive cache will be removed after 30 minutes.

- **Custom Configuration Action**

    ```csharp
    services.AddDatabaseFeatureManagement<MyFeatureStore>()
        .WithCacheService(options =>
        {
            options.SlidingExpiration = TimeSpan.FromMinutes(10);
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
        });
    ```

- **IConfiguration**

    ```csharp
    var cacheConfiguration = Configuration.GetSection(FeatureCacheOptions.Name);
    services.AddDatabaseFeatureManagement<MyFeatureStore>()
            .WithCacheService(cacheConfiguration);
    ```
    And in your `appsettings.json`:
    
    ```json
    {
        "FeatureCacheOptions": {
            "AbsoluteExpirationRelativeToNow": "01:00:00",
            "SlidingExpiration": "00:30:00"
        }
    }
    ```

#### Advanced Cache Configuration

The cache keys have a prefix (**FMDb_**) defined in the options (`FeatureCacheOptions.CachePrefix`). By default:

|     | Single feature | All features  |
| --- | -------------- | ------------- |
| Key | FMDb_MyFeature | FMDb_features |

  
Note that _"features"_ can be overridden when configuring cache. So you can have `"FMDb_your-custom-cache-key"`.

See the `FeatureCacheOptions` class for more cache-related settings.

> [!WARNING]
> Cache does not auto-refresh when feature values update directly in the database. Handle cache invalidation appropriately.


## Consumption

The basic form of feature management is checking if a feature flag is enabled and then performing actions based on the result.
This is done through the `IFeatureManager`'s `IsEnabledAsync` method.

```csharp
…
IFeatureManager featureManager;
…
if (await featureManager.IsEnabledAsync("MyFeature"))
{
    // Do something
}
```

See more [here][Feature Management Consumption].

### ASP.NET Core Integration

The database feature management library provides support in ASP.NET Core and MVC to enable common feature flag scenarios in web applications.

See more [here][Feature Management ASP.NET Core].


## Built-In Database Providers

### Entity Framework Core

For easy integration with Entity Framework Core, you can use the `FeatureManagement.Database.EntityFrameworkCore` package.
This package provides:

- A default, extendable `FeatureManagementDbContext` with pre-configured entities for features and feature settings.
- A default `FeatureStore` implementation of the `IFeatureStore` interface, which can be extended as needed.

#### Usage

First, install the package:

```sh
dotnet add package FeatureManagement.Database.EntityFrameworkCore
```

Then configure the services with the database provider you want:

```csharp
services.AddDatabaseFeatureManagement<FeatureStore>()
    .ConfigureDbContext<FeatureManagementDbContext>(builder => ...);
```

Using EF Core, you can work with different database providers which provide an extension method to the `services.AddDatabaseFeatureManagement<FeatureStore>()`:

| Database Provider | Package | Extension method |
| ----------------- | ------- | ---------------- |
| SQL Server | `FeatureManagement.Database.EntityFrameworkCore.SqlServer` | `UseSqlServer<FeatureManagementDbContext>(...);` |
| PostgreSQL | `FeatureManagement.Database.EntityFrameworkCore.PostgreSQL` | `UseNpgsql<FeatureManagementDbContext>(...);` |
| Sqlite | `FeatureManagement.Database.EntityFrameworkCore.Sqlite` | `UseSqlite<FeatureManagementDbContext>(...);` |
| MySql | `FeatureManagement.Database.EntityFrameworkCore.MySql` | `UseMySql<FeatureManagementDbContext>(...);` |


If you already have an existing DbContext and want to integrate it with EF Core, download the main package and then
you can inherit from `FeatureManagementDbContext`, so update your registration accordingly using your database provider (e.g. SQL Server):

```csharp
services.AddDbContext<MyDbContext>(builder => builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

services.AddDatabaseFeatureManagement<MyFeatureStore>();
```

> [!NOTE]
> When using a custom DbContext, ensure that `MyFeatureStore` also extends the default one to utilize the custom DbContext.


### Dapper

For easy integration with Dapper, you can use the `FeatureManagement.Database.Dapper` package.
This package provides:

- A default `FeatureStore` implementation of the `IFeatureStore` interface, which can be extended as needed.
- A `IDbConnectionFactory` for creating database connections.

#### Usage

First, install the package:

```sh
dotnet add package FeatureManagement.Database.Dapper
```

Then implement `IDbConnectionFactory` to create a connection string to connect to your database:

```csharp
public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlServerConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
```

And configure the services:

```csharp
services.AddDatabaseFeatureManagement<FeatureStore>()
    .UseDapper(new SqlServerConnectionFactory("Your_SqlServer_ConnString"));
```


### MongoDB

For easy integration with MongoDB, you can use the `FeatureManagement.Database.MongoDB` package.
This package provides:

- A default `FeatureStore` implementation of the `IFeatureStore` interface, which can be extended as needed.
- A `IMongoDBConnectionFactory` for creating database connections with default implementation.

- #### Usage

First, install the package:

```sh
dotnet add package FeatureManagement.Database.MongoDB
```

Then use the default `MongoDBConnectionFactory` or implement `IMongoDBConnectionFactory` to create a custom connection string to connect to your database,
and configure the services:

```csharp
services.AddDatabaseFeatureManagement<FeatureStore>()
    .UseMongoDB(...);
```


## Contributing

Please see [Contribution Guidelines](CONTRIBUTING.md) for more information.


[Feature Management]: https://github.com/microsoft/FeatureManagement-Dotnet
[Feature Management built-in filters]: https://learn.microsoft.com/azure/azure-app-configuration/feature-management-dotnet-reference#built-in-feature-filters
[Feature Management contextual filters]: https://learn.microsoft.com/azure/azure-app-configuration/feature-management-dotnet-reference#contextual-feature-filters
[Feature Management custom filters]: https://learn.microsoft.com/azure/azure-app-configuration/feature-management-dotnet-reference#implementing-a-feature-filter
[Feature Management Consumption]: https://learn.microsoft.com/azure/azure-app-configuration/feature-management-dotnet-reference#consumption
[Feature Management ASP.NET Core]: https://learn.microsoft.com/azure/azure-app-configuration/feature-management-dotnet-reference#aspnet-core-integration