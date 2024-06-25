// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FluentNHibernate.Cfg.Db;

namespace FeatureManagement.Database.NHibernate;

// TODO: make this public and overridable?

internal static class DatabaseConfigurer
{
    internal static IPersistenceConfigurer ConfigureDatabase(string connectionString, DatabaseType databaseType)
    {
        return databaseType switch
        {
            DatabaseType.SqlServer => ConfigureSqlServer(connectionString),
            DatabaseType.PostgreSQL => ConfigurePostgreSQL(connectionString),
            DatabaseType.SQLite => ConfigureSQLite(connectionString),
            DatabaseType.None => throw new ArgumentException("Use a valid database provider.", nameof(databaseType)),
            _ => throw new NotImplementedException("Database not supported by default. Implement your own.")
        };
    }

    private static IPersistenceConfigurer ConfigureSqlServer(string connectionString)
    {
        return MsSqlConfiguration.MsSql2012
            .ConnectionString(connectionString)
            .ShowSql();
    }

    private static IPersistenceConfigurer ConfigurePostgreSQL(string connectionString)
    {
        return PostgreSQLConfiguration.PostgreSQL83
            .ConnectionString(connectionString)
            .ShowSql();
    }

    private static IPersistenceConfigurer ConfigureSQLite(string connectionString)
    {
        return SQLiteConfiguration.Standard
            .ConnectionString(connectionString)
            .ShowSql();
    }
}