// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database;
using System.Diagnostics.CodeAnalysis;
using static WebApiApp.Features;

namespace WebApiApp;

public class FeatureStore : IFeatureStore
{
    private readonly IReadOnlyCollection<Feature> _features;

    public FeatureStore()
    {
        _features =
        [
            new Feature {
                Name = Weather,
                Settings = [new FeatureSettings { FilterType = FeatureFilterType.Percentage, Parameters = """{ "Value": 50 }""" }]
            }
        ];
    }

    public Task<Feature?> GetFeatureAsync([NotNull] string featureName)
    {
        return Task.FromResult(_features.SingleOrDefault(x => x.Name == featureName));
    }

    public Task<IReadOnlyCollection<Feature>> GetFeaturesAsync()
    {
        return Task.FromResult(_features);
    }
}