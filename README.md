# <img height="55" src="\build\icon.png" align="center"> .NET Database Feature Management


[![Build Status](https://github.com/teociaps/FeatureManagement.Database/actions/workflows/build.yml/badge.svg)](https://github.com/teociaps/FeatureManagement.Database/actions/workflows/build.yml)
[![Test Status](https://github.com/teociaps/FeatureManagement.Database/actions/workflows/test.yml/badge.svg)](https://github.com/teociaps/FeatureManagement.Database/actions/workflows/test.yml)

**.NET Feature Management Database** extends [Microsoft Feature Management](https://github.com/microsoft/FeatureManagement-Dotnet) for retrieving feature definitions from various databases.
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
* [Contributing](#contributing)


## Features

- **Database Integration**: Store and retrieve feature definitions from various databases.
- **Caching**: Built-in support for caching feature definitions to enhance performance and reduce database load.
- **Customizable**: Easily extend and customize to support additional storage solutions and unique requirements.
- **Seamless Integration**: Integrates smoothly with Microsoft Feature Management, enabling efficient database-backed feature flag management.


## Packages

| Package | NuGet Version |
| ------- | ------------- |
| [FeatureManagement.Database.Abstractions](https://www.nuget.org/packages/FeatureManagement.Database.Abstractions/) | [![NuGet Version](https://img.shields.io/nuget/v/FeatureManagement.Database.svg?style=flat)](https://www.nuget.org/packages/FeatureManagement.Database.Abstractions/)

**Package Purposes**

* _FeatureManagement.Database.Abstractions_
	* Standard functionalities for managing feature flags across various databases


## Getting Started

TODO explain better


The built-in `DatabaseFeatureDefinitionProvider` fetches feature definitions from the database. It relies on an implementation of the `IFeatureStore` interface to retrieve feature data and convert it into `FeatureDefinition` objects used by the feature management system.

### Entities

**Feature**: Represents a feature with its associated settings and parameters. Each feature has a _unique_ name, requirement type, and a collection of settings that define how the feature is enabled or disabled.

**FeatureSettings**: Contains the settings for a feature, including parameters that are typically stored in _JSON format_. These settings define the conditions under which a feature is enabled.
The parameters are based on [Feature Management filter configuration](https://github.com/microsoft/FeatureManagement-Dotnet?tab=readme-ov-file#built-in-feature-filters) + contextual(TODOfor built-in features; for custom feature filter see [how to configure it](https://github.com/microsoft/FeatureManagement-Dotnet?tab=readme-ov-file#implementing-a-feature-filter).

### Feature Store

The `IFeatureStore` interface is the core abstraction for retrieving feature data from a database.
Implement this interface to connect to your specific database (e.g., SQL Server, MongoDB, etc.).

``` csharp
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

    ``` csharp
    services.AddFeatureStore<MyFeatureStore>();
    services.AddDatabaseFeatureManagement();
    ```

    This approach allows for more modular and explicit registration of services.

* #### Register Feature Store and Feature Management in a Single Call

    For a more streamlined setup, you can register your custom `IFeatureStore` and add database feature management in one step:

    ``` csharp
    services.AddDatabaseFeatureManagement<MyFeatureStore>();
    ```

    This method simplifies the configuration by combining both registrations into a single call.

> [!NOTE]
> In the context of database solution the feature management services will be added as scoped services.

> [!IMPORTANT]
> To use database feature management you need to register an **IFeatureStore**.


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

See `FeatureCacheOptions` for more cache-related settings.

## Consumption

The basic form of feature management is checking if a feature flag is enabled and then performing actions based on the result.
This is done through the `IFeatureManager`'s `IsEnabledAsync` method.

``` csharp
…
IFeatureManager featureManager;
…
if (await featureManager.IsEnabledAsync("FeatureA"))
{
    // Do something
}
```

See more [here](https://github.com/microsoft/FeatureManagement-Dotnet?tab=readme-ov-file#consumption).

### ASP.NET Core Integration

The database feature management library provides support in ASP.NET Core and MVC to enable common feature flag scenarios in web applications.

See more [here](https://github.com/microsoft/FeatureManagement-Dotnet?tab=readme-ov-file#aspnet-core-integration).


## Contributing

We welcome contributions! Please see our [Contribution Guidelines](CONTRIBUTING.md) for more information.