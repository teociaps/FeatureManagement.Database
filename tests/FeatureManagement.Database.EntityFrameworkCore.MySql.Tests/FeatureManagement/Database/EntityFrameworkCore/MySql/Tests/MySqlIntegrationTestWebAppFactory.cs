// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using DotNet.Testcontainers.Builders;
using FeatureManagement.Database.EntityFrameworkCore.Tests;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Testcontainers.MySql;

namespace FeatureManagement.Database.EntityFrameworkCore.MySql.Tests;

public sealed class MySqlIntegrationTestWebAppFactory : IntegrationTestWebAppFactory<MySqlContainer>
{
    private readonly MySqlContainer _mySqlContainer = new MySqlBuilder()
        .WithName("mysql-test-container")
        .WithImage("mysql:latest")
        .WithDatabase("TestDb")
        .WithUsername("root")
        .WithPassword("mysqlpassword")
        .WithPrivileged(true)
        .WithPortBinding(MySqlBuilder.MySqlPort)
        .WithCleanUp(true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MySqlBuilder.MySqlPort))
        .Build();

    public MySqlIntegrationTestWebAppFactory()
    {
        _container = _mySqlContainer;
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddDatabaseFeatureManagement<CustomEFCoreFeatureStore>()
            .UseMySql<TestDbContext>(_mySqlContainer.GetConnectionString(),
                options => options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
    }
}