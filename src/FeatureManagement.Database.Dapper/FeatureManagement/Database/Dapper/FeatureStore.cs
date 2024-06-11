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
    /// The database connection.
    /// </summary>
    protected readonly IDbConnection DbConnection;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureStore"/> class.
    /// </summary>
    /// <param name="dbConnection">The database connection.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="dbConnection"/> is null.</exception>
    public FeatureStore(IDbConnection dbConnection)
    {
        DbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
    }

    /// <inheritdoc/>
    public async virtual Task<Feature> GetFeatureAsync([NotNull] string featureName)
    {
        const string Query = @"
            SELECT * FROM Features WHERE Name = @FeatureName;
            SELECT * FROM FeatureSettings WHERE FeatureId = (SELECT Id FROM Features WHERE Name = @FeatureName);
        ";

        await using var multi = await DbConnection.QueryMultipleAsync(Query, new { FeatureName = featureName });

        var feature = await multi.ReadSingleOrDefaultAsync<Feature>();
        if (feature is not null)
        {
            feature.Settings = (await multi.ReadAsync<FeatureSettings>()).ToList();
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

        await using var multi = await DbConnection.QueryMultipleAsync(Query);

        var features = (await multi.ReadAsync<Feature>()).ToList();
        var settings = (await multi.ReadAsync<FeatureSettings>()).ToList();

        foreach (var feature in features)
        {
            feature.Settings = settings.Where(s => s.FeatureId == feature.Id).ToList();
        }

        return features;
    }
}