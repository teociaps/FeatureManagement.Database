// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using static FeatureManagement.Database.Features;

namespace FeatureManagement.Database.EntityFrameworkCore.SqlServer.Tests;

public sealed class SqlServerFeatureStoreTests : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    public SqlServerFeatureStoreTests(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }

    [Fact]
    public async Task GetFeatureFromDatabaseUsingEfCore()
    {
        // Arrange
        var featureStore = _scope.ServiceProvider.GetRequiredService<IFeatureStore>();

        // Act
        var feature = await featureStore.GetFeatureAsync(FirstFeature);

        // Assert
        Assert.True(feature is not null);
        Assert.Equal(FirstFeature, feature.Name);
        Assert.True(feature.Settings.Any());
        Assert.Equal(FeatureFilterType.TimeWindow, feature.Settings.First().FilterType);
    }
}