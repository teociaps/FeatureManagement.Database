// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using ConsoleApp.FeatureFilters;
using FeatureManagement.Database;
using System.Diagnostics.CodeAnalysis;
using static ConsoleApp.Features;

namespace ConsoleApp;

public class FeatureStore : IFeatureStore
{
    private readonly IReadOnlyCollection<Feature> _features;

    public FeatureStore()
    {
        _features =
        [
            new Feature {
                Name = Beta,
                Settings = [new FeatureSettings { CustomFilterTypeName = UsernameFilter.Name, FilterType = FeatureFilterType.Custom, Parameters = """{ "AllowedUsernames": [ "Matthew" ] }""" }]
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