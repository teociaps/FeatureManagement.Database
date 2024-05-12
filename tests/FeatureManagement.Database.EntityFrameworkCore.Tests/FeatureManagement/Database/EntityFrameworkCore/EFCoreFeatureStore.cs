// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FeatureManagement.Database.EntityFrameworkCore;

public class EFCoreFeatureStore : IFeatureStore
{
    private readonly FeatureManagementDbContext _dbContext;

    public EFCoreFeatureStore(FeatureManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Feature> GetFeatureAsync([NotNull] string featureName)
    {
        return await _dbContext.Features.SingleOrDefaultAsync(x => x.Name == featureName);
    }

    public async Task<IReadOnlyCollection<Feature>> GetFeaturesAsync()
    {
        return await _dbContext.Features.ToListAsync();
    }
}