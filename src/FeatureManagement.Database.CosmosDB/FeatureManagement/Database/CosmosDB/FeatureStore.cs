// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace FeatureManagement.Database.CosmosDB;

// TODO: improve performances

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
    public virtual async Task<Feature> GetFeatureAsync(string featureName)
    {
        var featuresContainer = ConnectionFactory.GetFeaturesContainer();

        var query = new QueryDefinition("SELECT * FROM c WHERE c.Name = @FeatureName")
            .WithParameter("@FeatureName", featureName);

        var feature = await GetFirstOrDefaultAsync<Feature>(featuresContainer, query);
        if (feature is not null && !_options.UseSeparateContainers)
        {
            feature.Settings = await GetFeatureSettingsAsync(featuresContainer, feature.Id);
        }
        else if (feature is not null && _options.UseSeparateContainers)
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
        if (!_options.UseSeparateContainers)
        {
            foreach (var feature in features)
            {
                feature.Settings = await GetFeatureSettingsAsync(featuresContainer, feature.Id);
            }
        }
        else
        {
            var featureSettingsContainer = ConnectionFactory.GetFeatureSettingsContainer();
            var settings = await GetListAsync<FeatureSettings>(featureSettingsContainer, new QueryDefinition("SELECT * FROM c"));
            foreach (var feature in features)
            {
                feature.Settings = settings.Where(s => s.FeatureId == feature.Id).ToList();
            }
        }

        return features;
    }

    private static async Task<List<T>> GetListAsync<T>(Container container, QueryDefinition query)
    {
        var items = new List<T>();
        using (var feedIterator = container.GetItemQueryIterator<T>(query))
        {
            while (feedIterator.HasMoreResults)
            {
                var response = await feedIterator.ReadNextAsync();
                items.AddRange([..response]);
            }
        }
        return items;
    }

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

    private static async Task<List<FeatureSettings>> GetFeatureSettingsAsync(Container container, Guid featureId)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.FeatureId = @FeatureId")
            .WithParameter("@FeatureId", featureId);

        return await GetListAsync<FeatureSettings>(container, query);
    }
}