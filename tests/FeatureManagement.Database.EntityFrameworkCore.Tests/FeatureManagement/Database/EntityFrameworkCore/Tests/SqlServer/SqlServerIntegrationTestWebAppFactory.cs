// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using DotNet.Testcontainers.Builders;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace FeatureManagement.Database.EntityFrameworkCore.Tests;

public sealed class SqlServerIntegrationTestWebAppFactory : IntegrationTestWebAppFactory<MsSqlContainer>
{
    private readonly MsSqlContainer _sqlServerContainer = new MsSqlBuilder()
        .WithName("sqlserver-test-container")
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithPortBinding(MsSqlBuilder.MsSqlPort)
        .WithCleanUp(true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MsSqlBuilder.MsSqlPort))
        .Build();

    public SqlServerIntegrationTestWebAppFactory()
    {
        _container = _sqlServerContainer;
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddDatabaseFeatureManagement<CustomEFCoreFeatureStore>()
            .UseSqlServer<TestDbContext>(_sqlServerContainer.GetConnectionString());
    }
}