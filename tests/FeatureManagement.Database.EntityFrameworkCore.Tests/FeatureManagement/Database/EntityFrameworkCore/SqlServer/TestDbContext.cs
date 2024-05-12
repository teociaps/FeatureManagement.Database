// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;

namespace FeatureManagement.Database.EntityFrameworkCore.SqlServer;

public class TestDbContext : FeatureManagementDbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options)
    {
    }
}