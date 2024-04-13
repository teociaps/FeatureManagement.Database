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
                Name = "FirstFeature",
                RequirementType = Microsoft.FeatureManagement.RequirementType.All,
                Settings = [new FeatureSettings { FilterType = FeatureFilterType.TimeWindow, Parameters = """{"Start": "Mon, 01 May 2023 13:59:59 GMT", "End": "Sat, 01 July 2023 00:00:00 GMT"}""" }]
            },
            new Feature {
                Name = "SecondFeature",
                RequirementType = Microsoft.FeatureManagement.RequirementType.All,
                Settings = [new FeatureSettings { FilterType = FeatureFilterType.TimeWindow, Parameters = """{"Start": "Mon, 01 May 2023 13:59:59 GMT", "End": "Sat, 01 July 2023 00:00:00 GMT"}""" }]
            }
        ];
    }

    public Task<Feature> GetFeatureAsync([NotNull] string featureName)
    {
        return Task.FromResult(_features.SingleOrDefault(x => x.Name == featureName));
    }

    public async IAsyncEnumerable<Feature> GetFeaturesAsync()
    {
        await foreach (var feature in _features.ToAsyncEnumerable())
        {
            yield return feature;
        }
    }
}