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
    public virtual async Task<Feature> GetFeatureAsync([NotNull] string featureName)
    {
        const string Query = @"
            SELECT * FROM Features WHERE Name = @FeatureName;
            SELECT * FROM FeatureSettings WHERE FeatureId = (SELECT Id FROM Features WHERE Name = @FeatureName);
        ";

        using var connection = DbConnectionFactory.CreateConnection();

        await using var multiQuery = await connection.QueryMultipleAsync(Query, new { FeatureName = featureName });

        var feature = await multiQuery.ReadSingleOrDefaultAsync<Feature>();
        if (feature is not null)
        {
            feature.Settings = [.. (await multiQuery.ReadAsync<FeatureSettings>())];
        }

        return feature;
    }

    /// <inheritdoc/>
    public virtual async Task<Feature> GetFeatureAsync(Guid featureId)
    {
        const string Query = @"
            SELECT * FROM Features WHERE Id = @FeatureId;
            SELECT * FROM FeatureSettings WHERE FeatureId = @FeatureId;
        ";

        using var connection = DbConnectionFactory.CreateConnection();

        await using var multiQuery = await connection.QueryMultipleAsync(Query, new { FeatureId = featureId });

        var feature = await multiQuery.ReadSingleOrDefaultAsync<Feature>();
        if (feature is not null)
        {
            feature.Settings = [.. (await multiQuery.ReadAsync<FeatureSettings>())];
        }

        return feature;
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyCollection<Feature>> GetFeaturesAsync()
    {
        const string Query = @"
            SELECT * FROM Features;
            SELECT * FROM FeatureSettings WHERE FeatureId IN (SELECT Id FROM Features);
        ";

        using var connection = DbConnectionFactory.CreateConnection();

        await using var multiQuery = await connection.QueryMultipleAsync(Query);

        var features = (await multiQuery.ReadAsync<Feature>()).ToList();
        var settings = (await multiQuery.ReadAsync<FeatureSettings>()).ToList();

        foreach (var feature in features)
        {
            feature.Settings = [.. settings.Where(s => s.FeatureId == feature.Id)];
        }

        return features;
    }

    /// <inheritdoc/>
    public virtual async Task<Feature> CreateFeatureAsync([NotNull] Feature feature)
    {
        const string InsertFeatureQuery = @"
            INSERT INTO Features (Id, Name, RequirementType)
            VALUES (@Id, @Name, @RequirementType);
        ";

        const string InsertFeatureSettingsQuery = @"
            INSERT INTO FeatureSettings (Id, FeatureId, FilterType, Parameters, CustomFilterTypeName)
            VALUES (@Id, @FeatureId, @FilterType, @Parameters, @CustomFilterTypeName);
        ";

        using var connection = DbConnectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(InsertFeatureQuery, feature, transaction);

            if (feature.Settings?.Count > 0)
            {
                foreach (var setting in feature.Settings)
                {
                    await connection.ExecuteAsync(InsertFeatureSettingsQuery, setting, transaction);
                }
            }

            transaction.Commit();
            return feature;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<Feature> UpdateFeatureAsync([NotNull] Feature feature)
    {
        const string UpdateFeatureQuery = @"
            UPDATE Features
            SET Name = @Name, RequirementType = @RequirementType
            WHERE Id = @Id;
        ";

        const string DeleteFeatureSettingsQuery = @"
            DELETE FROM FeatureSettings WHERE FeatureId = @FeatureId;
        ";

        const string InsertFeatureSettingsQuery = @"
            INSERT INTO FeatureSettings (Id, FeatureId, FilterType, Parameters, CustomFilterTypeName)
            VALUES (@Id, @FeatureId, @FilterType, @Parameters, @CustomFilterTypeName);
        ";

        using var connection = DbConnectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(UpdateFeatureQuery, feature, transaction);

            await connection.ExecuteAsync(DeleteFeatureSettingsQuery, new { FeatureId = feature.Id }, transaction);

            if (feature.Settings?.Count > 0)
            {
                foreach (var setting in feature.Settings)
                {
                    await connection.ExecuteAsync(InsertFeatureSettingsQuery, setting, transaction);
                }
            }

            transaction.Commit();
            return feature;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task DeleteFeatureAsync(Guid featureId)
    {
        const string DeleteFeatureQuery = @"
            DELETE FROM Features WHERE Id = @FeatureId;
        ";

        const string DeleteFeatureSettingsQuery = @"
            DELETE FROM FeatureSettings WHERE FeatureId = @FeatureId;
        ";

        using var connection = DbConnectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(DeleteFeatureSettingsQuery, new { FeatureId = featureId }, transaction);
            await connection.ExecuteAsync(DeleteFeatureQuery, new { FeatureId = featureId }, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<FeatureSettings> GetFeatureSettingAsync(Guid featureSettingId)
    {
        const string Query = @"
            SELECT * FROM FeatureSettings WHERE Id = @FeatureSettingId;
        ";

        using var connection = DbConnectionFactory.CreateConnection();

        var featureSetting = await connection.QuerySingleOrDefaultAsync<FeatureSettings>(Query, new { FeatureSettingId = featureSettingId });

        return featureSetting;
    }

    /// <inheritdoc/>
    public virtual async Task<FeatureSettings> CreateFeatureSettingAsync([NotNull] FeatureSettings featureSetting)
    {
        const string InsertFeatureSettingQuery = @"
            INSERT INTO FeatureSettings (Id, FeatureId, FilterType, Parameters, CustomFilterTypeName)
            VALUES (@Id, @FeatureId, @FilterType, @Parameters, @CustomFilterTypeName);
        ";

        using var connection = DbConnectionFactory.CreateConnection();

        await connection.ExecuteAsync(InsertFeatureSettingQuery, featureSetting);

        return featureSetting;
    }

    /// <inheritdoc/>
    public virtual async Task<FeatureSettings> UpdateFeatureSettingAsync([NotNull] FeatureSettings featureSetting)
    {
        const string UpdateFeatureSettingQuery = @"
            UPDATE FeatureSettings
            SET FilterType = @FilterType, Parameters = @Parameters, CustomFilterTypeName = @CustomFilterTypeName
            WHERE Id = @Id;
        ";

        using var connection = DbConnectionFactory.CreateConnection();

        await connection.ExecuteAsync(UpdateFeatureSettingQuery, featureSetting);

        return featureSetting;
    }

    /// <inheritdoc/>
    public virtual async Task DeleteFeatureSettingAsync(Guid featureSettingId)
    {
        const string DeleteFeatureSettingQuery = @"
            DELETE FROM FeatureSettings WHERE Id = @FeatureSettingId;
        ";

        using var connection = DbConnectionFactory.CreateConnection();

        await connection.ExecuteAsync(DeleteFeatureSettingQuery, new { FeatureSettingId = featureSettingId });
    }
}