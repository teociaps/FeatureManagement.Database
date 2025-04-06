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
    private readonly MsSqlContainer _sqlServerContainer;

    public IntegrationTestWebAppFactory()
    {
        var containerName = GetUniqueContainerName("nhibernate-sqlserver-test-container");

        _sqlServerContainer = new MsSqlBuilder()
            .WithName(containerName)
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("SA_USERNAME", MsSqlBuilder.DefaultUsername)
            .WithEnvironment("SA_PASSWORD", MsSqlBuilder.DefaultPassword)
            .WithPortBinding(MsSqlBuilder.MsSqlPort)
            .WithCleanUp(true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MsSqlBuilder.MsSqlPort))
            .Build();
    }

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

    private static string GetUniqueContainerName(string baseName)
    {
        var framework = Environment.GetEnvironmentVariable("DOTNET_TARGET_FRAMEWORK") ?? "default";
        return $"{baseName}-{framework}-{Guid.NewGuid().ToString("N")[..8]}";
    }
}