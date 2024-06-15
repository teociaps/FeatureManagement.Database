// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace FeatureManagement.Database.MongoDB;

/// <summary>
/// MongoDB implementation of <see cref="IFeatureStore"/>.
/// </summary>
public class FeatureStore : IFeatureStore
{
    private readonly IMongoDbConnectionFactory _connectionFactory;

    /// <summary>
    /// The <see cref="Feature"/> collection.
    /// </summary>
    protected readonly IMongoCollection<Feature> FeatureCollection;

    /// <summary>
    /// The <see cref="FeatureSettings"/> collection.
    /// </summary>
    protected readonly IMongoCollection<FeatureSettings> FeatureSettingsCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureStore"/> class.
    /// </summary>
    /// <param name="connectionFactory">The MongoDB connection factory.</param>
    public FeatureStore(IMongoDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

        var database = GetDatabase();
        FeatureCollection = database.GetCollection<Feature>("Features");
        FeatureSettingsCollection = database.GetCollection<FeatureSettings>("FeatureSettings");
    }

    /// <inheritdoc/>
    public virtual async Task<Feature> GetFeatureAsync([NotNull] string featureName)
    {
        var filter = Builders<Feature>.Filter.Eq(f => f.Name, featureName);
        var feature = await FeatureCollection.Find(filter).FirstOrDefaultAsync();

        if (feature is not null)
        {
            var settingsFilter = Builders<FeatureSettings>.Filter.Eq(fs => fs.FeatureId, feature.Id);
            feature.Settings = await FeatureSettingsCollection.Find(settingsFilter).ToListAsync();
        }

        return feature;
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyCollection<Feature>> GetFeaturesAsync()
    {
        var features = await FeatureCollection.Find(_ => true).ToListAsync();

        foreach (var feature in features)
        {
            var settingsFilter = Builders<FeatureSettings>.Filter.Eq(fs => fs.FeatureId, feature.Id);
            feature.Settings = await FeatureSettingsCollection.Find(settingsFilter).ToListAsync();
        }

        return features;
    }

    private IMongoDatabase GetDatabase()
    {
        return _connectionFactory.GetDatabase();
    }
}