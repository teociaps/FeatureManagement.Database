// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

// Ignore Spelling: Mongo

using MongoDB.Driver;

namespace FeatureManagement.Database.MongoDB;

/// <summary>
/// Default implementation of <see cref="IMongoDbConnectionFactory"/>.
/// </summary>
public class MongoDBConnectionFactory : IMongoDbConnectionFactory
{
    private readonly string _connectionString;
    private readonly string _databaseName;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDBConnectionFactory"/> class.
    /// </summary>
    /// <param name="connectionString">The MongoDB connection string.</param>
    /// <param name="databaseName">The name of the database to use.</param>
    /// <exception cref="ArgumentNullException">Thrown if any provided parameter is null.</exception>
    public MongoDBConnectionFactory(string connectionString, string databaseName)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
    }

    /// <inheritdoc/>
    public virtual IMongoDatabase GetDatabase()
    {
        var client = new MongoClient(_connectionString);
        return client.GetDatabase(_databaseName);
    }
}