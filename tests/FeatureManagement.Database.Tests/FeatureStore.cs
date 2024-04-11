// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace FeatureManagement.Database.Tests;

public class FeatureStore : IFeatureStore
{
    private readonly Feature[] _features;

    public FeatureStore()
    {
        _features =
        [
            new Feature {
                Name = "TestFeature",
                RequirementType = Microsoft.FeatureManagement.RequirementType.All,
                Settings = [new FeatureSettings { FilterType = FeatureFilterType.TimeWindow, Parameters = """{"Start": "Mon, 01 May 2023 13:59:59 GMT", "End": "Sat, 01 July 2023 00:00:00 GMT"}""" }]
            }
        ];
    }

    public Task<Feature> GetFeatureAsync([NotNull] string featureName)
    {
        return Task.FromResult(_features.SingleOrDefault(x => x.Name == featureName));
    }
}