// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using NHibernate;
using NHibernate.Linq;
using System.Diagnostics.CodeAnalysis;

#if !NET8_0_OR_GREATER

using System.Collections.ObjectModel;

#endif

namespace FeatureManagement.Database.NHibernate;

/// <summary>
/// NHibernate implementation of <see cref="IFeatureStore"/>.
/// </summary>
public class FeatureStore : IFeatureStore
{
    /// <summary>
    ///The session factory used to create database connections.
    /// </summary>
    protected readonly ISessionFactory SessionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureStore"/> class.
    /// </summary>
    /// <param name="sessionFactory">The session factory used to create database connections.</param>
    public FeatureStore(ISessionFactory sessionFactory)
    {
        SessionFactory = sessionFactory;
    }

    /// <inheritdoc/>
    public virtual async Task<Feature> GetFeatureAsync([NotNull] string featureName)
    {
        using var session = SessionFactory.OpenSession();
        return await session.QueryOver<Feature>()
                            .Where(f => f.Name == featureName)
                            .Fetch(SelectMode.Fetch, f => f.Settings)
                            .SingleOrDefaultAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<Feature> GetFeatureAsync(Guid featureId)
    {
        using var session = SessionFactory.OpenSession();
        return await session.QueryOver<Feature>()
                            .Where(f => f.Id == featureId)
                            .Fetch(SelectMode.Fetch, f => f.Settings)
                            .SingleOrDefaultAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyCollection<Feature>> GetFeaturesAsync()
    {
        using var session = SessionFactory.OpenSession();
        var features = await session.QueryOver<Feature>()
                                    .Fetch(SelectMode.Fetch, f => f.Settings)
                                    .ListAsync();
#if NET8_0_OR_GREATER
        return features.AsReadOnly();
#else
        return new ReadOnlyCollection<Feature>(features);
#endif
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
        using var session = SessionFactory.OpenSession();
        using var transaction = session.BeginTransaction();
        try
        {
            await session.SaveAsync(feature);

            if (feature.Settings is not null)
            {
                foreach (var setting in feature.Settings)
                {
                    setting.Feature = feature; // Ensure the relationship is set
                    await session.SaveAsync(setting);
                }
            }

            await transaction.CommitAsync();
            return feature;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException($"Failed to create feature with name '{feature.Name}'.", ex);
        }
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
        using var session = SessionFactory.OpenSession();
        using var transaction = session.BeginTransaction();
        try
        {
            await session.UpdateAsync(feature);

            if (feature.Settings is not null)
            {
                foreach (var setting in feature.Settings)
                {
                    setting.Feature = feature; // Ensure the relationship is set
                    await session.MergeAsync(setting); // Handle new or existing settings
                }
            }

            await transaction.CommitAsync();
            return feature;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException($"Failed to update feature with name '{feature.Name}'.", ex);
        }
    }

    /// <inheritdoc/>
    public virtual async Task DeleteFeatureAsync(Guid featureId)
    {
        using var session = SessionFactory.OpenSession();
        using var transaction = session.BeginTransaction();
        try
        {
            var feature = await session.Query<Feature>()
                                       .Fetch(f => f.Settings)
                                       .SingleOrDefaultAsync(f => f.Id == featureId)
                                       ?? throw new KeyNotFoundException($"Feature with Id '{featureId}' not found.");

            if (feature.Settings is not null)
            {
                foreach (var setting in feature.Settings)
                {
                    await session.DeleteAsync(setting);
                }
            }

            await session.DeleteAsync(feature);

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException($"Failed to delete feature with Id '{featureId}'.", ex);
        }
    }

    /// <inheritdoc/>
    public virtual async Task<FeatureSettings> GetFeatureSettingAsync(Guid featureSettingId)
    {
        using var session = SessionFactory.OpenSession();
        return await session.QueryOver<FeatureSettings>()
                            .Where(fs => fs.Id == featureSettingId)
                            .SingleOrDefaultAsync();
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
        using var session = SessionFactory.OpenSession();
        await session.SaveAsync(featureSetting);
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
        using var session = SessionFactory.OpenSession();
        await session.UpdateAsync(featureSetting);
        return featureSetting;
    }

    /// <inheritdoc/>
    public virtual async Task DeleteFeatureSettingAsync(Guid featureSettingId)
    {
        using var session = SessionFactory.OpenSession();
        var featureSetting = await session.Query<FeatureSettings>()
                                          .SingleOrDefaultAsync(fs => fs.Id == featureSettingId)
                                          ?? throw new KeyNotFoundException($"Feature setting with Id '{featureSettingId}' not found.");

        await session.DeleteAsync(featureSetting);
    }
}