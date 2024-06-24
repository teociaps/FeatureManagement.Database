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
    /// Creates a new session factory to open database sessions.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>An instance of <see cref="ISessionFactory"/>.</returns>
    ISessionFactory CreateSessionFactory(string connectionString);


    /// <summary>
    /// Creates a new session factory to open database sessions.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="databaseType">
    /// The pre-configured type of database to use.
    /// <para />
    /// To use a custom database see <see cref="CreateSessionFactory(Action{NHibernateConfigurationOptions})"/>
    /// or implement a custom <see cref="INHibernateConnectionFactory"/>.
    /// </param>
    /// <returns>An instance of <see cref="ISessionFactory"/>.</returns>
    ISessionFactory CreateSessionFactory(string connectionString, DatabaseType databaseType);

    /// <summary>
    /// Creates a new session factory to open database sessions.
    /// </summary>
    /// <param name="configureOptions">An action to configure NHibernate options.</param>
    /// <returns>An instance of <see cref="ISessionFactory"/>.</returns>
    ISessionFactory CreateSessionFactory(Action<NHibernateConfigurationOptions> configureOptions);
}