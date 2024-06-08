// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using static FeatureManagement.Database.Features;

namespace FeatureManagement.Database.EntityFrameworkCore.Tests;

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
                Id = Guid.Parse("7C81E846-DC77-4AFF-BF03-8DD8BB2D3194"),
                Name = FirstFeature,
                RequirementType = Microsoft.FeatureManagement.RequirementType.All,
            },
            new()
            {
                Id = Guid.Parse("D3C82992-2F12-4008-9376-DA37695A2747"),
                Name = SecondFeature,
                RequirementType = Microsoft.FeatureManagement.RequirementType.All,
            }
        ];

        List<FeatureSettings> settings =
        [
            new()
            {
                Id = Guid.Parse("672DC1BD-9C5B-44CE-8461-234B262A8395"),
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