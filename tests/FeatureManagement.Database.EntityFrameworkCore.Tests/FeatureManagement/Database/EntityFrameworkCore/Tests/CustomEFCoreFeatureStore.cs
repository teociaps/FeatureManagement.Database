// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

namespace FeatureManagement.Database.EntityFrameworkCore.Tests;

public class CustomEFCoreFeatureStore : FeatureStore
{
    public CustomEFCoreFeatureStore(TestDbContext dbContext) : base(dbContext)
    {
    }
}