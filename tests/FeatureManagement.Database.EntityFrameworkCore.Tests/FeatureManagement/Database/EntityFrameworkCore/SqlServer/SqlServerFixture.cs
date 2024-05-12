// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Testcontainers.MsSql;

namespace FeatureManagement.Database.EntityFrameworkCore.SqlServer;

//TODO: add application factory

public sealed class SqlServerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServerContainer = new MsSqlBuilder()
        .WithName("sqlserver-test-container")
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithEnvironment("ACCEPT_EULA", "Y")
        .Build();

    public string ConnectionString => _sqlServerContainer.GetConnectionString();

    public Task InitializeAsync()
    {
        return _sqlServerContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _sqlServerContainer.DisposeAsync().AsTask();
    }
}