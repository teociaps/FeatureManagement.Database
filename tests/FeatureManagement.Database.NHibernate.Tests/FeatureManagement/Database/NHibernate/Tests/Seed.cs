// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using NHibernate;
using static FeatureManagement.Database.Features;

namespace FeatureManagement.Database.NHibernate.Tests;

internal static class Seed
{
    internal static async Task SeedData(ISessionFactory sessionFactory)
    {
        using var session = sessionFactory.OpenSession();
        using var transaction = session.BeginTransaction();

        // Create your feature and feature settings entities
        List<Feature> features =
        [
            new()
            {
                Id = Guid.Parse("7C81E846-DC77-4AFF-BF03-8DD8BB2D3194"),
                Name = FirstFeature,
                RequirementType = Microsoft.FeatureManagement.RequirementType.All
            },
            new()
            {
                Id = Guid.Parse("D3C82992-2F12-4008-9376-DA37695A2747"),
                Name = SecondFeature,
                RequirementType = Microsoft.FeatureManagement.RequirementType.All
            }
        ];

        var featureSetting = new FeatureSettings
        {
            Id = Guid.Parse("672DC1BD-9C5B-44CE-8461-234B262A8395"),
            CustomFilterTypeName = null,
            FilterType = FeatureFilterType.TimeWindow,
            Parameters = "{\"Start\": \"Mon, 01 May 2023 13:59:59 GMT\", \"End\": \"Sat, 01 July 2023 00:00:00 GMT\"}",
            //Feature = features[0]
        };

        // Add settings to the feature
        features[0].Settings = [ featureSetting ];

        // Save entities to the database
        await session.SaveAsync(featureSetting);
        await session.SaveAsync(features[0]);
        await session.SaveAsync(features[1]);

        // Commit the transaction
        await transaction.CommitAsync();
    }
}