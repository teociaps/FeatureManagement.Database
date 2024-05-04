// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

namespace FeatureManagement.Database;

/// <summary>
/// Represents the feature cache-related settings used to cache data from the database.
/// </summary>
public class FeatureCacheOptions
{
    /// <summary>
    /// The name of this options class.
    /// </summary>
    public const string Name = nameof(FeatureCacheOptions);

    /// <summary>
    /// Gets or sets an absolute expiration date for the cache entry.
    /// </summary>
    public DateTimeOffset? AbsoluteExpiration { get; set; }

    /// <summary>
    /// Gets or sets an absolute expiration time, relative to now.
    /// </summary>
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

    /// <summary>
    /// Gets or sets how long a cache entry can be inactive (e.g. not accessed) before
    /// it will be removed. This will not extend the entry lifetime beyond the absolute
    /// expiration (if set).
    /// </summary>
    /// <remarks>
    /// Default value is 30 minutes.
    /// </remarks>
    public TimeSpan? SlidingExpiration { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Provides configuration settings for cache key names related to feature data.
    /// </summary>
    public KeyNameConfig KeyNames { get; set; } = new();

    /// <summary>
    /// Represents the configuration for cache key names.
    /// </summary>
    public class KeyNameConfig
    {
        /// <summary>
        /// The cache key name used to store all features data.
        /// </summary>
        public string AllFeatures { get; set; } = "features";
    };
}