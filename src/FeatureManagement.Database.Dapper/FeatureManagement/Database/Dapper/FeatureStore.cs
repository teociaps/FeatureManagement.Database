// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Dapper;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace FeatureManagement.Database.Dapper;

/// <summary>
/// Dapper implementation of <see cref="IFeatureStore"/>.
/// </summary>
public class FeatureStore : IFeatureStore
{
    /// <summary>
    /// The database connection factory.
    /// </summary>
    protected readonly IDbConnectionFactory DbConnectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureStore"/> class.
    /// </summary>
    /// <param name="dbConnectionFactory">The database connection factory.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="dbConnectionFactory"/> is null.</exception>
    public FeatureStore(IDbConnectionFactory dbConnectionFactory)
    {
        DbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
    }

    /// <inheritdoc/>
    public async virtual Task<Feature> GetFeatureAsync([NotNull] string featureName)
    {
        const string Query = @"
            SELECT * FROM Features WHERE Name = @FeatureName;
            SELECT * FROM FeatureSettings WHERE FeatureId = (SELECT Id FROM Features WHERE Name = @FeatureName);
        ";

        using var connection = DbConnectionFactory.CreateConnection();

#if NET6_0_OR_GREATER
        await using var multiQuery = await connection.QueryMultipleAsync(Query, new { FeatureName = featureName });
#else
        using var multiQuery = await connection.QueryMultipleAsync(Query, new { FeatureName = featureName });
#endif

        var feature = await multiQuery.ReadSingleOrDefaultAsync<Feature>();
        if (feature is not null)
        {
            feature.Settings = (await multiQuery.ReadAsync<FeatureSettings>()).ToList();
        }

        return feature;
    }

    /// <inheritdoc/>
    public async virtual Task<IReadOnlyCollection<Feature>> GetFeaturesAsync()
    {
        const string Query = @"
            SELECT * FROM Features;
            SELECT * FROM FeatureSettings WHERE FeatureId IN (SELECT Id FROM Features);
        ";

        using var connection = DbConnectionFactory.CreateConnection();

#if NET6_0_OR_GREATER
        await using var multiQuery = await connection.QueryMultipleAsync(Query);
#else
        using var multiQuery = await connection.QueryMultipleAsync(Query);
#endif

        var features = (await multiQuery.ReadAsync<Feature>()).ToList();
        var settings = (await multiQuery.ReadAsync<FeatureSettings>()).ToList();

        foreach (var feature in features)
        {
            feature.Settings = settings.Where(s => s.FeatureId == feature.Id).ToList();
        }

        return features;
    }
}