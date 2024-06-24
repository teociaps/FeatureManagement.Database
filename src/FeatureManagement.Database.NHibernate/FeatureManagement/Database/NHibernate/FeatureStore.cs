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
    ///The session factory used to crea database connections.
    /// </summary>
    protected readonly ISessionFactory SessionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureStore"/> class.
    /// </summary>
    /// <param name="sessionFactory">The session factory used to crea database connections.</param>
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
}