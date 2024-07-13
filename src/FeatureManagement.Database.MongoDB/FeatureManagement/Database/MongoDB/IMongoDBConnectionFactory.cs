// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using MongoDB.Driver;

namespace FeatureManagement.Database.MongoDB;

/// <summary>
/// Represents a factory interface for creating connections to a MongoDB database.
/// </summary>
public interface IMongoDBConnectionFactory
{
    /// <summary>
    /// Gets a MongoDB database instance.
    /// </summary>
    /// <returns>An instance of <see cref="IMongoDatabase"/>.</returns>
    IMongoDatabase GetDatabase();
}
