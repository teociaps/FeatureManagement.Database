// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using NHibernate;

namespace FeatureManagement.Database.NHibernate;

/// <summary>
/// Represents a factory interface for creating session factory used to open database connections.
/// </summary>
public interface INHibernateConnectionFactory
{
    /// <summary>
    /// Creates a new session factory to open database sessions using the provided connection string.
    /// </summary>
    /// <returns>An instance of <see cref="ISessionFactory"/>.</returns>
    ISessionFactory CreateSessionFactory(string connectionString);
}