using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using static Dapper.SqlMapper;
using static FeatureManagement.Database.Features;

namespace FeatureManagement.Database.Dapper.Tests;

public class DapperFeatureStoreFixture : IDisposable
{
    public IFeatureStore FeatureStore { get; }

    private readonly SqliteConnection _connection;
    private const string _SqliteConnectionString = "DataSource=TestDb;Mode=Memory;Cache=Shared";

    public DapperFeatureStoreFixture()
    {
        AddTypeHandler(new GuidTypeHandler());

        // Initialize the shared database and feature store
        var factory = new SqliteConnectionFactory(_SqliteConnectionString);
        var services = new ServiceCollection();
        services.AddDatabaseFeatureManagement<FeatureStore>()
            .UseDapper(factory);
        var serviceProvider = services.BuildServiceProvider();

        FeatureStore = serviceProvider.GetRequiredService<IFeatureStore>();
        _connection = (SqliteConnection)factory.CreateConnection();

        // Setup the database schema and test data
        SetupTestDataAsync().GetAwaiter().GetResult();
    }

    private async Task SetupTestDataAsync()
    {
        await CreateTablesAsync();
        await InsertTestDataAsync();
    }

    private async Task CreateTablesAsync()
    {
        await _connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS Features (
                Id TEXT PRIMARY KEY,
                Name TEXT NOT NULL,
                RequirementType INTEGER NOT NULL
            )");

        await _connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS FeatureSettings (
                Id TEXT PRIMARY KEY,
                CustomFilterTypeName TEXT,
                FilterType INTEGER NOT NULL,
                Parameters TEXT,
                FeatureId TEXT NOT NULL,
                FOREIGN KEY (FeatureId) REFERENCES Features(Id)
            )");
    }

    internal static readonly Guid _firstFeatureId = Guid.Parse("7C81E846-DC77-4AFF-BF03-8DD8BB2D3194");

    private async Task InsertTestDataAsync()
    {
        List<Feature> features =
        [
            new()
            {
                Id = _firstFeatureId,
                Name = FirstFeature,
                RequirementType = RequirementType.All,
            },
            new()
            {
                Id = Guid.Parse("D3C82992-2F12-4008-9376-DA37695A2747"),
                Name = SecondFeature,
                RequirementType = RequirementType.All,
            },
            new()
            {
                Id = Guid.Parse("0ECE94E6-75A0-4257-9E8E-180A297FA7D8"),
                Name = FeatureToUpdate,
                RequirementType = RequirementType.Any,
            },
            new()
            {
                Id = Guid.Parse("DD323F34-2ED5-49C1-ACB7-D3B4A99BAD4B"),
                Name = FeatureToDelete,
                RequirementType = RequirementType.All,
            }
        ];

        List<FeatureSettings> settings =
        [
            new()
            {
                Id = Guid.Parse("672DC1BD-9C5B-44CE-8461-234B262A8395"),
                FeatureId = features[0].Id,
                FilterType = FeatureFilterType.TimeWindow,
                Parameters = """{"Start": "Mon, 01 May 2023 13:59:59 GMT", "End": "Sat, 01 July 2023 00:00:00 GMT"}"""
            },
            new()
            {
                Id = Guid.Parse("8190CC07-7499-46F0-93FC-FE45BF828DF3"),
                FeatureId = features[1].Id,
                FilterType = FeatureFilterType.TimeWindow,
                Parameters = """{"Start": "Mon, 01 May 2023 13:59:59 GMT", "End": "Sat, 01 July 2023 00:00:00 GMT"}"""
            }
        ];

        // Add settings to the feature
        features[0].Settings = [settings[0]];
        features[1].Settings = [settings[1]];

        // Insert Features data
        foreach (var feature in features)
        {
            await _connection.ExecuteAsync(@"
            INSERT INTO Features (Id, Name, RequirementType)
            VALUES (@Id, @Name, @RequirementType)",
            feature);
        }

        // Insert FeatureSettings data
        foreach (var setting in settings)
        {
            await _connection.ExecuteAsync(@"
                INSERT INTO FeatureSettings (Id, CustomFilterTypeName, FilterType, Parameters, FeatureId)
                VALUES (@Id, @CustomFilterTypeName, @FilterType, @Parameters, @FeatureId)",
                setting);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _connection?.Dispose();
    }
}