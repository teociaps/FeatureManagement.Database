// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using static FeatureManagement.Database.Features;

namespace FeatureManagement.Database.EntityFrameworkCore.SqlServer;

public class TestDbContext : FeatureManagementDbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        List<Feature> features =
        [
            new()
            {
                Id = Guid.NewGuid(),
                Name = FirstFeature,
                RequirementType = Microsoft.FeatureManagement.RequirementType.All,
            }
        ];

        List<FeatureSettings> settings =
        [
            new()
            {
                Id = Guid.NewGuid(),
                FeatureId = features[0].Id,
                FilterType = FeatureFilterType.TimeWindow,
                Parameters = """{"Start": "Mon, 01 May 2023 13:59:59 GMT", "End": "Sat, 01 July 2023 00:00:00 GMT"}"""
            }
        ];

        modelBuilder.Entity<Feature>().HasData(features);
        modelBuilder.Entity<FeatureSettings>().HasData(settings);

        base.OnModelCreating(modelBuilder);
    }
}