// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace FeatureManagement.Database.MongoDB.Tests;

public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDBContainer = new MongoDbBuilder()
        .WithName("mongodb-test-container")
        .WithImage("mongo:latest")
        .WithUsername(null)
        .WithPassword(null)
        .WithPortBinding(MongoDbBuilder.MongoDbPort)
        .WithCleanUp(true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MongoDbBuilder.MongoDbPort))
        .Build();

    private const string _Database = "TestDb";

    internal string ConnectionString => _mongoDBContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _mongoDBContainer.StartAsync();

        var client = new MongoClient(ConnectionString);
        var database = client.GetDatabase(_Database);
        await Seed.SeedDataAsync(database);
    }

    public new async Task DisposeAsync()
    {
        await _mongoDBContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(ConfigureServices);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddDatabaseFeatureManagement<FeatureStore>()
            .UseMongoDB(ConnectionString, _Database);
    }
}