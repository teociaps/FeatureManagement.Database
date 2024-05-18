// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FeatureManagement.Database.EntityFrameworkCore;

public class EFCoreFeatureStore : IFeatureStore
{
    private readonly TestDbContext _dbContext;

    public EFCoreFeatureStore(TestDbContext dbContext)
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