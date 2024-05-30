# <img height="55" src="\build\icon.png" align="center"> .NET Database Feature Management


[![Build Status](https://github.com/teociaps/FeatureManagement.Database/actions/workflows/build.yml/badge.svg)](https://github.com/teociaps/FeatureManagement.Database/actions/workflows/build.yml)
[![Test Status](https://github.com/teociaps/FeatureManagement.Database/actions/workflows/test.yml/badge.svg)](https://github.com/teociaps/FeatureManagement.Database/actions/workflows/test.yml)

**.NET Feature Management Database** extends [Feature Management] for retrieving feature definitions from various databases.
It includes abstractions and default implementations to facilitate easy integration with your .NET applications.


## Supported .NET Versions

| Version | Status |
| ------- | ------ |
| .NET 6  | ![Badge](https://img.shields.io/badge/Status-Supported-brightgreen) |
| .NET 7  | ![Badge](https://img.shields.io/badge/Status-Supported-brightgreen) |
| .NET 8  | ![Badge](https://img.shields.io/badge/Status-Supported-brightgreen) |


## Index

* [Features](#features)
* [Packages](#packages)
* [Getting Started](#getting-started)
    * [Feature Store](#feature-store)
    * [Service Registration](#service-registration)
    * [Configure Cache](#configure-cache)
* [Consumption](#consumption)
    * [ASP.NET Core Integration](#asp.net-core-integration)


## Features

- **Database Integration**: store and retrieve feature definitions from various databases.
- **Caching**: built-in support for caching feature definitions to enhance performance and reduce database load.
- **Customizable**: easily extend and customize to support additional storage solutions and unique requirements.
- **Seamless Integration**: integrates smoothly with Microsoft Feature Management, enabling efficient database-backed feature flag management.


## Packages

| Package | NuGet Version |
| ------- | ------------- |
| [FeatureManagement.Database.Abstractions](https://www.nuget.org/packages/FeatureManagement.Database.Abstractions/) | [![NuGet Version](https://img.shields.io/nuget/v/FeatureManagement.Database.svg?style=flat)](https://www.nuget.org/packages/FeatureManagement.Database.Abstractions/)

**Package Purposes**

* _FeatureManagement.Database.Abstractions_
	* Standard functionalities for managing feature flags across various databases


## Getting Started

.NET Feature Management Database allows you to manage feature definitions stored in a database.
The built-in `DatabaseFeatureDefinitionProvider` retrieves these definitions and converts them into `FeatureDefinition` objects used by the feature management system.
This setup relies on an implementation of the `IFeatureStore` interface to access the database.

### Entities

Two primary entities are pre-configured for database feature management:

- **Feature**: represents a feature with its associated settings. Each feature has a unique name, a requirement type, and a collection of settings that define how the feature is enabled or disabled.

- **FeatureSettings**: contains the settings for a feature and these define the conditions under which a feature is enabled.
The parameters are stored in JSON format and based on Feature Management [built-in feature filter][Feature Management built-in filters] or [contextual feature filter][Feature Management contextual filters] configuration, and can include [custom feature filter][Feature Management custom filters] configuration.

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

### Service Registration

Database feature management relies on .NET Core dependency injection.
Registering the feature management services can be done using the following approaches:

* #### Register Feature Store and Feature Management Separately

    First, register your custom `IFeatureStore` implementation and then add database feature management:

    ```csharp
    services.AddFeatureStore<MyFeatureStore>();
    services.AddDatabaseFeatureManagement();
    ```

    This approach allows for more modular and explicit registration of services.

* #### Register Feature Store and Feature Management in a Single Call

    For a more streamlined setup, you can register your custom `IFeatureStore` and add database feature management in one step:

    ```csharp
    services.AddDatabaseFeatureManagement<MyFeatureStore>();
    ```

    This method simplifies the configuration by combining both registrations into a single call.

> [!NOTE]
> In the context of database solutions, the feature management services will be added as scoped services.

> [!IMPORTANT]
> To use database feature management, you need to register an **IFeatureStore**.

### Configure Cache

To improve performance and reduce database load, you can configure caching for the feature store.
The `WithCacheService` method provides several ways to configure caching:

* #### Using Default Options
  
    To register the cache service with default options:

    ```csharp
    services.AddDatabaseFeatureManagement<MyFeatureStore>()
            .WithCacheService();
    ```

    > By default, the inactive cache will be removed after 30 minutes.

* #### Using Custom Configuration Action

    To register the cache service with custom options:

    ```csharp
    services.AddDatabaseFeatureManagement<MyFeatureStore>()
            .WithCacheService(options =>
            {
                options.SlidingExpiration = TimeSpan.FromMinutes(10);
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            });
    ```

* #### Using Configuration Section

    To register the cache service using settings from a configuration section:

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

The cache keys have a prefix defined in the options (`FeatureCacheOptions.CachePrefix`).

- For a single feature, the default cache key will be the name of that feature (prefix included).
    Example:
  
    ```text
    MyFeature => FMDb_MyFeature
    ```

- For all features, the default cache key will be "features" combined with the prefix:
    
    ```text
    All features => FMDb_features
    ```
    
    That _"features"_ can be overridden using one of the methods above. So you can have `"FMDb_your-own-cache-key"`.

See the `FeatureCacheOptions` class for more cache-related settings.

> [!WARNING]
> When a feature value is updated in the database, the cache does not automatically clean up or refresh.
> Ensure to handle cache invalidation appropriately in such scenarios to keep the cache in sync with the database.


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


## Contributing

Please see our [Contribution Guidelines](CONTRIBUTING.md) for more information.


[Feature Management]: https://github.com/microsoft/FeatureManagement-Dotnet
[Feature Management built-in filters]: https://github.com/microsoft/FeatureManagement-Dotnet?tab=readme-ov-file#built-in-feature-filters
[Feature Management contextual filters]: https://github.com/microsoft/FeatureManagement-Dotnet?tab=readme-ov-file#contextual-feature-filters
[Feature Management custom filters]: https://github.com/microsoft/FeatureManagement-Dotnet?tab=readme-ov-file#implementing-a-feature-filter
[Feature Management Consumption]: https://github.com/microsoft/FeatureManagement-Dotnet?tab=readme-ov-file#consumption
[Feature Management ASP.NET Core]: https://github.com/microsoft/FeatureManagement-Dotnet?tab=readme-ov-file#aspnet-core-integration