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
    public MySqlIntegrationTestWebAppFactory()
    {
        var containerName = GetUniqueContainerName("mysql-test-container");

        _container = new MySqlBuilder()
            .WithName(containerName)
            .WithImage("mysql:latest")
            .WithDatabase("TestDb")
            .WithUsername("root")
            .WithPassword("mysqlpassword")
            .WithPrivileged(true)
            .WithPortBinding(MySqlBuilder.MySqlPort, assignRandomHostPort: true)
            .WithCleanUp(true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MySqlBuilder.MySqlPort))
            .Build();
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddDatabaseFeatureManagement<CustomEFCoreFeatureStore>()
            .UseMySql<TestDbContext>(_container.GetConnectionString(),
                options => options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
    }
}