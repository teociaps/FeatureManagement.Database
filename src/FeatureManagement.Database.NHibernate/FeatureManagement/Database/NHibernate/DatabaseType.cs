// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

namespace FeatureManagement.Database.NHibernate;

/// <summary>
/// Specifies the pre-configured types of databases supported.
/// </summary>
public enum DatabaseType
{
    /// <summary>
    /// No database specified. You need to select any other option.
    /// </summary>
    None,

    /// <summary>
    /// Microsoft SQL Server database.
    /// </summary>
    SqlServer,

    /// <summary>
    /// PostgreSQL database.
    /// </summary>
    PostgreSQL,

    /// <summary>
    /// SQLite database.
    /// </summary>
    SQLite
}
