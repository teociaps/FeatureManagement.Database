// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Testcontainers.CosmosDb;

namespace FeatureManagement.Database.CosmosDB.Tests;

public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly CosmosDbContainer _cosmosDbContainer = new CosmosDbBuilder()
            .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
            .WithExposedPort(8081)
            .WithPortBinding(8081, true)
            .WithEnvironment("AZURE_COSMOS_EMULATOR_PARTITION_COUNT", "2")
            .WithEnvironment("AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE", "false")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8081))
            .Build();

    internal string ConnectionString => _cosmosDbContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _cosmosDbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var connectionFactory = scope.ServiceProvider.GetRequiredService<ICosmosDBConnectionFactory>();
        var cosmosDbOptions = scope.ServiceProvider.GetRequiredService<IOptions<CosmosDBOptions>>();

        await Seed.SeedDataAsync(cosmosDbOptions.Value, connectionFactory);
    }

    public new async Task DisposeAsync()
    {
        await _cosmosDbContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(ConfigureServices);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddDatabaseFeatureManagement<FeatureStore>()
            .UseCosmosDB(options =>
            {
                options.EndpointUri = $"https://localhost:{CosmosDbBuilder.CosmosDbPort}";
                options.AccountKey = CosmosDbBuilder.DefaultAccountKey;
                options.DatabaseName = "TestDatabase";
                options.UseSeparateContainers = false;
            }, clientOptions =>
            {
                clientOptions.ConnectionMode = ConnectionMode.Gateway;
                clientOptions.MaxRetryAttemptsOnRateLimitedRequests = 10;
                clientOptions.MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(30);
                clientOptions.HttpClientFactory = () =>
                {
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                    return new HttpClient(handler);
                };
            });
    }
}