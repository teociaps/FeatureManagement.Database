// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using Testcontainers.MsSql;

namespace FeatureManagement.Database.NHibernate.Tests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServerContainer = new MsSqlBuilder()
        .WithName("sqlserver-test-container")
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithPortBinding(MsSqlBuilder.MsSqlPort)
        .WithCleanUp(true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MsSqlBuilder.MsSqlPort))
        .Build();

    public async Task InitializeAsync()
    {
        await _sqlServerContainer.StartAsync();

        using var scope = Services.CreateScope();
        var sessionFactory = scope.ServiceProvider.GetRequiredService<ISessionFactory>();

        await Seed.SeedData(sessionFactory);
    }

    public new async Task DisposeAsync()
    {
        await _sqlServerContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(ConfigureServices);
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddDatabaseFeatureManagement<FeatureStore>()
            .UseNHibernate(_sqlServerContainer.GetConnectionString(), new SqlServerConnectionFactory());
    }
}