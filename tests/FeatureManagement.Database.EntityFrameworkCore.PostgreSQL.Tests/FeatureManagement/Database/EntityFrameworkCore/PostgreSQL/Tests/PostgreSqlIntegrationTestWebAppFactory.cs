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
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithName("postgresql-test-container")
        .WithImage("postgres:latest")
        .WithPortBinding(PostgreSqlBuilder.PostgreSqlPort)
        .WithCleanUp(true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(PostgreSqlBuilder.PostgreSqlPort))
        .Build();

    public PostgreSqlIntegrationTestWebAppFactory()
    {
        _container = _postgreSqlContainer;
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddDatabaseFeatureManagement<CustomEFCoreFeatureStore>()
            .UseNpgsql<TestDbContext>(_postgreSqlContainer.GetConnectionString(),
                options => options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
    }
}