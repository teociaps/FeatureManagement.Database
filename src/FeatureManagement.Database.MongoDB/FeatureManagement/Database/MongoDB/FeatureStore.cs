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
    private readonly IMongoDBConnectionFactory _connectionFactory;

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
    public FeatureStore(IMongoDBConnectionFactory connectionFactory)
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
    public virtual async Task<Feature> GetFeatureAsync(Guid featureId)
    {
        var filter = Builders<Feature>.Filter.Eq(f => f.Id, featureId);
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

    /// <inheritdoc/>
    public virtual async Task<Feature> CreateFeatureAsync([NotNull] Feature feature)
    {
#if NET6_0_OR_GREATER
       ArgumentNullException.ThrowIfNull(feature);
#else
        if (feature is null)
            throw new ArgumentNullException(nameof(feature));
#endif
        try
        {
            await FeatureCollection.InsertOneAsync(feature);

            if (feature.Settings?.Count > 0)
            {
                foreach (var setting in feature.Settings)
                {
                    setting.FeatureId = feature.Id;
                }
                await FeatureSettingsCollection.InsertManyAsync(feature.Settings);
            }
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category is ServerErrorCategory.DuplicateKey)
        {
            throw new InvalidOperationException($"Feature with name '{feature.Name}' already exists.", ex);
        }

        return feature;
    }

    /// <inheritdoc/>
    public virtual async Task<Feature> UpdateFeatureAsync([NotNull] Feature feature)
    {
#if NET6_0_OR_GREATER
       ArgumentNullException.ThrowIfNull(feature);
#else
        if (feature is null)
            throw new ArgumentNullException(nameof(feature));
#endif
        var filter = Builders<Feature>.Filter.Eq(f => f.Id, feature.Id);
        var result = await FeatureCollection.ReplaceOneAsync(filter, feature);

        if (result.MatchedCount == 0)
            throw new KeyNotFoundException($"Feature with Id '{feature.Id}' not found.");

        if (feature.Settings?.Count > 0)
        {
            var settingsFilter = Builders<FeatureSettings>.Filter.Eq(fs => fs.FeatureId, feature.Id);
            await FeatureSettingsCollection.DeleteManyAsync(settingsFilter);

            foreach (var setting in feature.Settings)
            {
                setting.FeatureId = feature.Id;
            }
            await FeatureSettingsCollection.InsertManyAsync(feature.Settings);
        }

        return feature;
    }

    /// <inheritdoc/>
    public virtual async Task DeleteFeatureAsync(Guid featureId)
    {
        var featureFilter = Builders<Feature>.Filter.Eq(f => f.Id, featureId);
        var feature = await FeatureCollection.Find(featureFilter).FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException($"Feature with Id '{featureId}' not found.");

        var settingsFilter = Builders<FeatureSettings>.Filter.Eq(fs => fs.FeatureId, feature.Id);
        await FeatureSettingsCollection.DeleteManyAsync(settingsFilter);

        var result = await FeatureCollection.DeleteOneAsync(featureFilter);

        if (result.DeletedCount == 0)
            throw new KeyNotFoundException($"Feature with Id '{featureId}' not found.");
    }

    /// <inheritdoc/>
    public virtual async Task<FeatureSettings> GetFeatureSettingAsync(Guid featureSettingId)
    {
        var filter = Builders<FeatureSettings>.Filter.Eq(fs => fs.Id, featureSettingId);
        return await FeatureSettingsCollection.Find(filter).SingleOrDefaultAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<FeatureSettings> CreateFeatureSettingAsync([NotNull] FeatureSettings featureSetting)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(featureSetting);
#else
        if (featureSetting is null)
            throw new ArgumentNullException(nameof(featureSetting));
#endif
        try
        {
            await FeatureSettingsCollection.InsertOneAsync(featureSetting);
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category is ServerErrorCategory.DuplicateKey)
        {
            throw new InvalidOperationException($"Feature setting with Id '{featureSetting.Id}' already exists.", ex);
        }

        return featureSetting;
    }

    /// <inheritdoc/>
    public virtual async Task<FeatureSettings> UpdateFeatureSettingAsync([NotNull] FeatureSettings featureSetting)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(featureSetting);
#else
        if (featureSetting is null)
            throw new ArgumentNullException(nameof(featureSetting));
#endif
        var filter = Builders<FeatureSettings>.Filter.Eq(fs => fs.Id, featureSetting.Id);
        var result = await FeatureSettingsCollection.ReplaceOneAsync(filter, featureSetting);

        if (result.MatchedCount == 0)
            throw new KeyNotFoundException($"Feature setting with Id '{featureSetting.Id}' not found.");

        return featureSetting;
    }

    /// <inheritdoc/>
    public virtual async Task DeleteFeatureSettingAsync(Guid featureSettingId)
    {
        var filter = Builders<FeatureSettings>.Filter.Eq(fs => fs.Id, featureSettingId);
        var result = await FeatureSettingsCollection.DeleteOneAsync(filter);

        if (result.DeletedCount == 0)
            throw new KeyNotFoundException($"Feature setting with Id '{featureSettingId}' not found.");
    }

    private IMongoDatabase GetDatabase()
    {
        return _connectionFactory.GetDatabase();
    }
}