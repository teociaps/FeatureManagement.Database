// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.EntityFrameworkCore.SqlServer;

namespace FeatureManagement.Database.EntityFrameworkCore;

public class CustomEFCoreFeatureStore : FeatureStore
{
    public CustomEFCoreFeatureStore(TestDbContext dbContext) : base(dbContext)
    {
    }
}