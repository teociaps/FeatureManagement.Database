// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static FeatureManagement.Database.Features;

namespace FeatureManagement.Database.EntityFrameworkCore.SqlServer.Tests;

public sealed class SqlServerFeatureStoreTests : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture;

    public SqlServerFeatureStoreTests(SqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task TestFeatureWithEfCore()
    {
        // Assert/Act
        IServiceCollection services = new ServiceCollection();

        services.AddDatabaseFeatureManagement<EFCoreFeatureStore>();
        services.AddFeatureManagementDbContext<TestDbContext>(o => o.UseSqlServer(_fixture.ConnectionString));
        var serviceProvider = services.BuildServiceProvider();

        var dbContext = serviceProvider.GetRequiredService<TestDbContext>();
        await dbContext.Database.MigrateAsync();
        dbContext.Features.Add(new Feature
        {
            Name = FirstFeature,
            RequirementType = Microsoft.FeatureManagement.RequirementType.All,
            Settings = [new FeatureSettings { FilterType = FeatureFilterType.TimeWindow, Parameters = """{"Start": "Mon, 01 May 2023 13:59:59 GMT", "End": "Sat, 01 July 2023 00:00:00 GMT"}""" }]
        });

        var featureStore = serviceProvider.GetRequiredService<IFeatureStore>();

        var feature = await featureStore.GetFeatureAsync(FirstFeature);

        // Assert
        Assert.True(feature is not null);
        Assert.Equal(FirstFeature, feature.Name);
        Assert.True(feature.Settings.Any());
        Assert.Equal(FeatureFilterType.TimeWindow, feature.Settings.First().FilterType);
    }
}