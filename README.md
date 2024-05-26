# <img height="55" src="\build\icon.png" align="center"> .NET Database Feature Management


[![Build Status](https://github.com/teociaps/FeatureManagement.Database/actions/workflows/build.yml/badge.svg)](https://github.com/teociaps/FeatureManagement.Database/actions/workflows/build.yml)
[![Test Status](https://github.com/teociaps/FeatureManagement.Database/actions/workflows/test.yml/badge.svg)](https://github.com/teociaps/FeatureManagement.Database/actions/workflows/test.yml)

**.NET Feature Management Database** extends [Microsoft Feature Management](https://github.com/microsoft/FeatureManagement-Dotnet) for retrieving feature definitions from various databases.
It includes abstractions and default implementations to facilitate easy integration with your .NET applications.


## Index

* [Features](#features)
* [Packages](#packages)
* [Configuration](#configuration)
	* [Feature Store](#feature-store)
	* [Configure Cache](#configure-cache)
* [Supported .NET Versions](#supported-net-versions)
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


## Configuration

TODO explain better
- **Feature**: Represents a feature with its settings and parameters.
- **FeatureSettings** with parameters (JSON): Settings associated with features.
- **DatabaseFeatureDefinitionProvider**: Provider to fetch feature definitions from the database.

### Feature Store

The feature store is responsible for fetching feature definitions from the database.
Implement your own `IFeatureStore` to integrate with your preferred database.

``` csharp
public class MyFeatureStore : IFeatureStore
{
    // Implementation to fetch feature definitions from your database
}
```

### Service Registration

Database feature management relies on .NET Core dependency injection. Registering the feature management services can be done using the following approaches:

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

> !IMPORTANT
> To use database feature management you need to register an **IFeatureStore**.


### Configure Cache

Caching feature definitions can enhance performance and reduce database load.
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
    var cacheConfiguration = Configuration.GetSection("FeatureCacheOptions");
    services.AddDatabaseFeatureManagement<MyFeatureStore>()
            .WithCacheService(cacheConfiguration);
    ```

See `FeatureCacheOptions` for more cache-related settings.


## Supported .NET Versions

| Version | Status |
| ------- | ------ |
| .NET 6  | ![Badge](https://img.shields.io/badge/Status-Supported-brightgreen) |
| .NET 7  | ![Badge](https://img.shields.io/badge/Status-Supported-brightgreen) |
| .NET 8  | ![Badge](https://img.shields.io/badge/Status-Supported-brightgreen) |


## Contributing

We welcome contributions! Please see our [Contribution Guidelines](CONTRIBUTING.md) for more information.