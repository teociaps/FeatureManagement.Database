// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using System.Data;

namespace FeatureManagement.Database.Dapper;

/// <summary>
/// Represents a factory interface for creating database connections.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates a new database connection.
    /// </summary>
    /// <returns>An instance of <see cref="IDbConnection"/>.</returns>
    IDbConnection CreateConnection();
}