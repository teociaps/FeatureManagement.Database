// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using DotNet.Testcontainers.Builders;
using FeatureManagement.Database.EntityFrameworkCore.Tests;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Testcontainers.MsSql;

namespace FeatureManagement.Database.EntityFrameworkCore.SqlServer.Tests;

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
            .UseSqlServer<TestDbContext>(_sqlServerContainer.GetConnectionString(),
                options => options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
    }
}