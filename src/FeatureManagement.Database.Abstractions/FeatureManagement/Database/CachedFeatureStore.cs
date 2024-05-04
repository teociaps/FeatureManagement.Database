// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace FeatureManagement.Database;

/// <summary>
/// Implementation of <see cref="IFeatureStore"/> used to cache the features.
/// </summary>
public class CachedFeatureStore : IFeatureStore
{
    private readonly IFeatureStore _featureStore;
    private readonly IDistributedCache _cache;
    private readonly FeatureCacheOptions _cacheOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachedFeatureStore"/> class.
    /// </summary>
    /// <param name="featureStore">The concrete feature store.</param>
    /// <param name="cache">The cache service.</param>
    /// <param name="options">The cache options.</param>
    public CachedFeatureStore(IFeatureStore featureStore, IDistributedCache cache, IOptions<FeatureCacheOptions> options)
    {
        _featureStore = featureStore ?? throw new ArgumentNullException(nameof(featureStore));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc/>
    public async Task<Feature> GetFeatureAsync([NotNull] string featureName)
    {
        var feature = await GetCacheAsync<Feature>(featureName);
        if (feature is not null)
            return feature;

        feature = await _featureStore.GetFeatureAsync(featureName);
        if (feature is not null)
            await SetCacheAsync(featureName, feature);

        return feature;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<Feature>> GetFeaturesAsync()
    {
        var features = await GetCacheAsync<IReadOnlyCollection<Feature>>(_cacheOptions.KeyNames.AllFeatures);
        if (features is not null)
            return features;

        features = await _featureStore.GetFeaturesAsync();
        if (features is { Count: > 0 })
            await SetCacheAsync(_cacheOptions.KeyNames.AllFeatures, features);

        return features;
    }

    #region Private

    private async Task<TData> GetCacheAsync<TData>(string key) where TData : class
    {
        var cachedData = await _cache.GetAsync(key);

        if (cachedData is null)
            return null;

        return await JsonSerializer.DeserializeAsync<TData>(new MemoryStream(cachedData));
    }

    private async Task SetCacheAsync<TData>(string key, TData data) where TData : class
    {
        await _cache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(data), new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = _cacheOptions.AbsoluteExpiration,
            AbsoluteExpirationRelativeToNow = _cacheOptions.AbsoluteExpirationRelativeToNow,
            SlidingExpiration = _cacheOptions.SlidingExpiration
        });
    }

    #endregion Private
}