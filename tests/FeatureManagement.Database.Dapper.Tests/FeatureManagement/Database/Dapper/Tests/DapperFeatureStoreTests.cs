// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using static Dapper.SqlMapper;

namespace FeatureManagement.Database.Dapper.Tests;

public class DapperFeatureStoreTests
{
    private const string _SqliteConnectionString = "DataSource=TestDb;Mode=Memory;Cache=Shared";

    public DapperFeatureStoreTests()
    {
        AddTypeHandler(new GuidTypeHandler());
    }

    [Fact]
    public async Task TestFeatureStoreWithSqlite()
    {
        var factory = new SqliteConnectionFactory(_SqliteConnectionString);

        var services = new ServiceCollection();
        services.AddDatabaseFeatureManagement<FeatureStore>()
            .UseDapper(factory);

        var serviceProvider = services.BuildServiceProvider();

        await RunFeatureStoreTestsAsync(serviceProvider, factory);
    }

    #region Private

    private static async Task RunFeatureStoreTestsAsync(IServiceProvider serviceProvider, IDbConnectionFactory factory)
    {
        // Arrange
        var featureStore = serviceProvider.GetRequiredService<IFeatureStore>();

        using var connection = factory.CreateConnection();
        await SetupTestData((SqliteConnection)connection);

        // Act
        var features = await featureStore.GetFeaturesAsync();
        var feature = await featureStore.GetFeatureAsync("Feature1");

        // Assert
        Assert.NotEmpty(features);
        Assert.NotNull(feature);
        Assert.Equal("Feature1", feature.Name);
    }

    private static async Task SetupTestData(SqliteConnection connection)
    {
        await CreateTables(connection);

        await InsertTestData(connection);
    }

    private static async Task CreateTables(SqliteConnection connection)
    {
        // Create Features table
        await connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS Features (
                Id TEXT PRIMARY KEY,
                Name TEXT NOT NULL,
                RequirementType INTEGER NOT NULL
            )");

        // Create FeatureSettings table
        await connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS FeatureSettings (
                Id TEXT PRIMARY KEY,
                CustomFilterTypeName TEXT,
                FilterType INTEGER NOT NULL,
                Parameters TEXT,
                FeatureId TEXT NOT NULL,
                FOREIGN KEY (FeatureId) REFERENCES Features(Id)
            )");
    }

    private static async Task InsertTestData(SqliteConnection connection)
    {
        var feature = new Feature
        {
            Id = Guid.NewGuid(),
            Name = "Feature1",
            RequirementType = RequirementType.All
        };
        var settings = new List<FeatureSettings>()
        {
            new() {
                Id = Guid.NewGuid(),
                FilterType = FeatureFilterType.AlwaysOn,
                FeatureId = feature.Id
            }
        };

        // Insert Features data
        await connection.ExecuteAsync(@"
            INSERT INTO Features (Id, Name, RequirementType)
            VALUES (@Id, @Name, @RequirementType)",
            feature);

        // Insert FeatureSettings data
        foreach (var setting in settings)
        {
            await connection.ExecuteAsync(@"
                INSERT INTO FeatureSettings (Id, CustomFilterTypeName, FilterType, Parameters, FeatureId)
                VALUES (@Id, @CustomFilterTypeName, @FilterType, @Parameters, @FeatureId)",
                setting);
        }
    }

    #endregion Private
}