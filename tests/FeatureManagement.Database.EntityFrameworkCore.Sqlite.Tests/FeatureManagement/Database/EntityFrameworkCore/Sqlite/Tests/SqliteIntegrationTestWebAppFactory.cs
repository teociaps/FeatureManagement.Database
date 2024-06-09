// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.EntityFrameworkCore.Tests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace FeatureManagement.Database.EntityFrameworkCore.Sqlite.Tests;

public class SqliteIntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string _InMemoryDatabaseName = "TestDb";
    private const string _ConnectionString = $"DataSource={_InMemoryDatabaseName};Mode=Memory;Cache=Shared";

    private SqliteConnection _connection;

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
        _connection = new SqliteConnection(_ConnectionString);
        await _connection.OpenAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await _connection.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(ConfigureServices);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.RemoveAll(typeof(DbContextOptions<TestDbContext>));

        services.AddDatabaseFeatureManagement<CustomEFCoreFeatureStore>()
            .UseSqlite<TestDbContext>(_ConnectionString,
                options => options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
    }
}