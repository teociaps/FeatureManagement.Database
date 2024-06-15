// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.Data.Sqlite;
using System.Data;

namespace FeatureManagement.Database.Dapper;

public class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }
}