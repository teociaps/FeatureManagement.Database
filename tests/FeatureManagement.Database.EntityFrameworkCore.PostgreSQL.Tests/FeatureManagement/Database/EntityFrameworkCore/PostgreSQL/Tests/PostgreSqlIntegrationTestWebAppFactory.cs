// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using DotNet.Testcontainers.Builders;
using FeatureManagement.Database.EntityFrameworkCore.Tests;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Testcontainers.PostgreSql;

namespace FeatureManagement.Database.EntityFrameworkCore.PostgreSQL.Tests;

public sealed class PostgreSqlIntegrationTestWebAppFactory : IntegrationTestWebAppFactory<PostgreSqlContainer>
{
    public PostgreSqlIntegrationTestWebAppFactory()
    {
        var containerName = GetUniqueContainerName("postgresql-test-container");

        _container = new PostgreSqlBuilder()
            .WithName(containerName)
            .WithImage("postgres:latest")
            .WithPortBinding(PostgreSqlBuilder.PostgreSqlPort)
            .WithCleanUp(true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(PostgreSqlBuilder.PostgreSqlPort))
            .Build();
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddDatabaseFeatureManagement<CustomEFCoreFeatureStore>()
            .UseNpgsql<TestDbContext>(_container.GetConnectionString(),
                options => options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
    }
}