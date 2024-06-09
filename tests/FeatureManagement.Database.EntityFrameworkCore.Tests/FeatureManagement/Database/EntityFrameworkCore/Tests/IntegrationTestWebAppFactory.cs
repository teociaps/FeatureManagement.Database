// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FeatureManagement.Database.EntityFrameworkCore.Tests;

public abstract class IntegrationTestWebAppFactory<TContainer>
    : WebApplicationFactory<Program>
    , IAsyncLifetime
    where TContainer : IContainer
{
    protected TContainer _container;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(ConfigureServices);
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        services.RemoveAll(typeof(DbContextOptions<TestDbContext>));
    }
}