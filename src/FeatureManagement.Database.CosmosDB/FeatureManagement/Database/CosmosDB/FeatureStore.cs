// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace FeatureManagement.Database.CosmosDB;

/// <summary>
/// Cosmos DB implementation of <see cref="IFeatureStore"/>.
/// </summary>
public class FeatureStore : IFeatureStore
{
    private readonly CosmosDBOptions _options;

    /// <summary>
    /// The Cosmos DB connection factory.
    /// </summary>
    protected readonly ICosmosDBConnectionFactory ConnectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureStore"/> class.
    /// </summary>
    /// <param name="cosmosDbConnectionFactory">The Cosmos DB connection factory.</param>
    /// <param name="options">The Cosmos DB options.</param>
    public FeatureStore(ICosmosDBConnectionFactory cosmosDbConnectionFactory, IOptions<CosmosDBOptions> options)
    {
        ConnectionFactory = cosmosDbConnectionFactory ?? throw new ArgumentNullException(nameof(cosmosDbConnectionFactory));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc/>
    public virtual async Task<Feature> GetFeatureAsync([NotNull] string featureName)
    {
        var featuresContainer = ConnectionFactory.GetFeaturesContainer();

        var query = new QueryDefinition("SELECT * FROM c WHERE c.Name = @FeatureName")
            .WithParameter("@FeatureName", featureName);

        var feature = await GetFirstOrDefaultAsync<Feature>(featuresContainer, query);
        if (feature is not null && _options.UseSeparateContainers)
        {
            var featureSettingsContainer = ConnectionFactory.GetFeatureSettingsContainer();
            feature.Settings = await GetFeatureSettingsAsync(featureSettingsContainer, feature.Id);
        }

        return feature;
    }

    /// <inheritdoc/>
    public virtual async Task<Feature> GetFeatureAsync(Guid featureId)
    {
        var featuresContainer = ConnectionFactory.GetFeaturesContainer();

        var query = new QueryDefinition("SELECT * FROM c WHERE c.Id = @FeatureId")
            .WithParameter("@FeatureId", featureId);

        var feature = await GetFirstOrDefaultAsync<Feature>(featuresContainer, query);
        if (feature is not null && _options.UseSeparateContainers)
        {
            var featureSettingsContainer = ConnectionFactory.GetFeatureSettingsContainer();
            feature.Settings = await GetFeatureSettingsAsync(featureSettingsContainer, feature.Id);
        }

        return feature;
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyCollection<Feature>> GetFeaturesAsync()
    {
        var featuresContainer = ConnectionFactory.GetFeaturesContainer();

        var query = new QueryDefinition("SELECT * FROM c");

        var features = await GetListAsync<Feature>(featuresContainer, query);
        if (features.Count > 0 && _options.UseSeparateContainers)
        {
            var featureSettingsContainer = ConnectionFactory.GetFeatureSettingsContainer();

            var settings = await GetFeatureSettingsAsync(featureSettingsContainer, features.ConvertAll(f => f.Id));
            foreach (var feature in features)
            {
                feature.Settings = [.. settings.Where(s => s.FeatureId == feature.Id)];
            }
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
        var container = ConnectionFactory.GetFeaturesContainer();

        // Remove settings from the feature entity if using separate containers
        if (_options.UseSeparateContainers && feature.Settings is not null)
        {
            var settingsContainer = ConnectionFactory.GetFeatureSettingsContainer();
            foreach (var setting in feature.Settings)
            {
                setting.FeatureId = feature.Id;
                await settingsContainer.CreateItemAsync(setting, new PartitionKey(setting.FeatureId.ToString()));
            }
            feature.Settings = null;
        }

        await container.CreateItemAsync(feature, new PartitionKey(feature.Name));
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
        var container = ConnectionFactory.GetFeaturesContainer();

        // Remove settings from the feature entity if using separate containers
        if (_options.UseSeparateContainers && feature.Settings is not null)
        {
            var settingsContainer = ConnectionFactory.GetFeatureSettingsContainer();
            foreach (var setting in feature.Settings)
            {
                setting.FeatureId = feature.Id;
                await settingsContainer.UpsertItemAsync(setting, new PartitionKey(setting.FeatureId.ToString()));
            }
            feature.Settings = null;
        }

        await container.UpsertItemAsync(feature, new PartitionKey(feature.Name));
        return feature;
    }

    /// <inheritdoc/>
    public virtual async Task DeleteFeatureAsync(Guid featureId)
    {
        var container = ConnectionFactory.GetFeaturesContainer();
        var feature = await GetFeatureAsync(featureId)
            ?? throw new InvalidOperationException($"Feature with Id '{featureId}' does not exist.");

        await container.DeleteItemAsync<Feature>(featureId.ToString(), new PartitionKey(feature.Name));

        if (_options.UseSeparateContainers)
        {
            var settingsContainer = ConnectionFactory.GetFeatureSettingsContainer();
            var settings = await GetFeatureSettingsAsync(settingsContainer, feature.Id);
            foreach (var setting in settings)
            {
                await settingsContainer.DeleteItemAsync<FeatureSettings>(setting.Id.ToString(), new PartitionKey(setting.FeatureId.ToString()));
            }
        }
    }

    /// <inheritdoc/>
    public virtual async Task<FeatureSettings> GetFeatureSettingAsync(Guid featureSettingId)
    {
        var container = ConnectionFactory.GetFeaturesContainer();

        if (_options.UseSeparateContainers)
        {
            container = ConnectionFactory.GetFeatureSettingsContainer();
        }

        var query = new QueryDefinition("SELECT * FROM c WHERE c.Id = @FeatureSettingId")
            .WithParameter("@FeatureSettingId", featureSettingId);

        var featureSetting = await GetFirstOrDefaultAsync<FeatureSettings>(container, query);

        if (featureSetting is not null && _options.UseSeparateContainers)
        {
            var feature = await GetFeatureAsync(featureSetting.FeatureId);
            if (feature is not null)
            {
                featureSetting.Feature = feature;
            }
        }
        else if (featureSetting is null && !_options.UseSeparateContainers)
        {
            var feature = await GetFeatureAsync(featureSettingId);
            featureSetting = feature?.Settings?.FirstOrDefault(s => s.Id == featureSettingId);
        }

        return featureSetting;
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

        var container = ConnectionFactory.GetFeaturesContainer();

        if (_options.UseSeparateContainers)
        {
            container = ConnectionFactory.GetFeatureSettingsContainer();
            await container.CreateItemAsync(featureSetting, new PartitionKey(featureSetting.FeatureId.ToString()));
        }
        else
        {
            var feature = await GetFeatureAsync(featureSetting.FeatureId);
            if (feature is not null)
            {
                feature.Settings ??= [];
                feature.Settings.Add(featureSetting);
                await container.UpsertItemAsync(feature, new PartitionKey(feature.Name));
            }
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

        var container = ConnectionFactory.GetFeaturesContainer();

        if (_options.UseSeparateContainers)
        {
            container = ConnectionFactory.GetFeatureSettingsContainer();
            await container.UpsertItemAsync(featureSetting, new PartitionKey(featureSetting.FeatureId.ToString()));
        }
        else
        {
            var feature = await GetFeatureAsync(featureSetting.FeatureId);
            if (feature is not null)
            {
                var existingSetting = feature.Settings?.FirstOrDefault(s => s.Id == featureSetting.Id);
                if (existingSetting is not null)
                {
                    feature.Settings.Remove(existingSetting);
                }
                feature.Settings ??= [];
                feature.Settings.Add(featureSetting);
                await container.UpsertItemAsync(feature, new PartitionKey(feature.Name));
            }
        }

        return featureSetting;
    }

    /// <inheritdoc/>
    public virtual async Task DeleteFeatureSettingAsync(Guid featureSettingId)
    {
        var container = ConnectionFactory.GetFeaturesContainer();

        if (_options.UseSeparateContainers)
        {
            container = ConnectionFactory.GetFeatureSettingsContainer();
            var featureSetting = await GetFeatureSettingAsync(featureSettingId)
                ?? throw new InvalidOperationException($"Feature setting with Id '{featureSettingId}' does not exist.");

            await container.DeleteItemAsync<FeatureSettings>(featureSettingId.ToString(), new PartitionKey(featureSetting.FeatureId.ToString()));
        }
        else
        {
            var feature = await GetFeatureByFeatureSettingIdAsync(featureSettingId);
            if (feature is not null)
            {
                feature.Settings.Remove(feature.Settings.First(s => s.Id == featureSettingId));
                await container.UpsertItemAsync(feature, new PartitionKey(feature.Name));
            }
        }
    }

    #region Private

    private static async Task<T> GetFirstOrDefaultAsync<T>(Container container, QueryDefinition query)
    {
        using (var feedIterator = container.GetItemQueryIterator<T>(query))
        {
            if (feedIterator.HasMoreResults)
            {
                var response = await feedIterator.ReadNextAsync();
                return response.FirstOrDefault();
            }
        }

        return default;
    }

    private static async Task<List<T>> GetListAsync<T>(Container container, QueryDefinition query)
    {
        var items = new List<T>();
        using (var feedIterator = container.GetItemQueryIterator<T>(query))
        {
            while (feedIterator.HasMoreResults)
            {
                var response = await feedIterator.ReadNextAsync();
                items.AddRange(response);
            }
        }
        return items;
    }

    private static async Task<List<FeatureSettings>> GetFeatureSettingsAsync(Container container, List<Guid> featureIds)
    {
        var items = new List<FeatureSettings>();
        foreach (var featureId in featureIds)
            items.AddRange(await GetFeatureSettingsAsync(container, featureId));

        return items;
    }

    private static async Task<List<FeatureSettings>> GetFeatureSettingsAsync(Container container, Guid featureId)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.FeatureId = @FeatureId")
            .WithParameter("@FeatureId", featureId);

        return await GetListAsync<FeatureSettings>(container, query);
    }

    private async Task<Feature> GetFeatureByFeatureSettingIdAsync(Guid featureSettingId)
    {
        var container = ConnectionFactory.GetFeatureSettingsContainer();

        var query = new QueryDefinition("SELECT * FROM c WHERE c.Id = @FeatureSettingId")
            .WithParameter("@FeatureSettingId", featureSettingId);

        var featureSetting = await GetFirstOrDefaultAsync<FeatureSettings>(container, query)
            ?? throw new InvalidOperationException($"Feature setting with Id '{featureSettingId}' does not exist.");

        return await GetFeatureAsync(featureSetting.FeatureId);
    }

    #endregion Private
}