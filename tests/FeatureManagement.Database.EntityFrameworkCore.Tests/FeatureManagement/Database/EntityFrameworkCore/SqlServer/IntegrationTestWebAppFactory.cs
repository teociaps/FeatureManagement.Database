// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;

namespace FeatureManagement.Database.EntityFrameworkCore.SqlServer;

public class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>
    , IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServerContainer = new MsSqlBuilder()
        .WithName("sqlserver-test-container")
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithPortBinding(MsSqlBuilder.MsSqlPort)
        .WithCleanUp(true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MsSqlBuilder.MsSqlPort))
        .Build();

    internal string ConnectionString => _sqlServerContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _sqlServerContainer.StartAsync();
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _sqlServerContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Development);

        builder.ConfigureServices(services =>
        {
            services.AddDatabaseFeatureManagement<EFCoreFeatureStore>();

            // Ensure existing DbContextOptions are removed if necessary
            services.RemoveAll(typeof(DbContextOptions<TestDbContext>));

            // Add the DbContext with the SQL Server connection string from the container
            services.AddFeatureManagementDbContext<TestDbContext>(_sqlServerContainer.GetConnectionString());
        });
    }
}