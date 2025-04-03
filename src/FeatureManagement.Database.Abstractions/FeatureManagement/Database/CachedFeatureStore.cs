// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

#if NET9_0_OR_GREATER
using Microsoft.Extensions.Caching.Hybrid;
#else

using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;

#endif

namespace FeatureManagement.Database;

/// <summary>
/// Implementation of <see cref="IFeatureStore"/> used to cache the features.
/// </summary>
public class CachedFeatureStore : IFeatureStore
{
    private readonly IFeatureStore _featureStore;
#if NET9_0_OR_GREATER
    private readonly HybridCache _cache;
#else
    private readonly IDistributedCache _cache;

    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerOptions.Default)
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

#endif
    private readonly FeatureCacheOptions _cacheOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachedFeatureStore"/> class.
    /// </summary>
    /// <param name="featureStore">The concrete feature store.</param>
    /// <param name="cache">The cache service.</param>
    /// <param name="options">The cache options.</param>
    public CachedFeatureStore(IFeatureStore featureStore,
#if NET9_0_OR_GREATER
        HybridCache cache,
#else
        IDistributedCache cache,
#endif
        IOptions<FeatureCacheOptions> options)
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
        var cacheKey = FeatureCacheOptions.CachePrefix + key;

#if NET9_0_OR_GREATER
        return await _cache.GetOrCreateAsync(cacheKey, static (_) => ValueTask.FromResult(default(TData)));
#else
        var cachedData = await _cache.GetAsync(cacheKey);

        if (cachedData is null)
            return null;

        return await JsonSerializer.DeserializeAsync<TData>(new MemoryStream(cachedData), _jsonOptions);
#endif
    }

    private Task SetCacheAsync<TData>(string key, TData data) where TData : class
    {
        var cacheKey = FeatureCacheOptions.CachePrefix + key;

#if NET9_0_OR_GREATER
        TimeSpan? expiration = null;
        if (_cacheOptions.AbsoluteExpirationRelativeToNow.HasValue)
        {
            expiration = _cacheOptions.AbsoluteExpirationRelativeToNow.Value;
        }
        else if (_cacheOptions.SlidingExpiration.HasValue)
        {
            // Note: HybridCache treats 'Expiration' more like an absolute TTL from creation/access.
            // Using SlidingExpiration here effectively sets an absolute expiry based on this duration.
            expiration = _cacheOptions.SlidingExpiration.Value;
        }
        else if (_cacheOptions.AbsoluteExpiration.HasValue && _cacheOptions.AbsoluteExpiration > DateTimeOffset.UtcNow)
        {
            expiration = _cacheOptions.AbsoluteExpiration.Value - DateTimeOffset.UtcNow;
        }

        var entryOptions = new HybridCacheEntryOptions
        {
            Expiration = expiration > TimeSpan.Zero ? expiration : null
        };

        return _cache.SetAsync(cacheKey, data, entryOptions).AsTask();
#else
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = _cacheOptions.AbsoluteExpiration,
            AbsoluteExpirationRelativeToNow = _cacheOptions.AbsoluteExpirationRelativeToNow,
            SlidingExpiration = _cacheOptions.SlidingExpiration
        };
        return _cache.SetAsync(cacheKey, JsonSerializer.SerializeToUtf8Bytes(data, _jsonOptions), options);
#endif
    }

    #endregion Private
}