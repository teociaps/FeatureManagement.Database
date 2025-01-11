// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;
using static FeatureManagement.Database.Abstractions.Features;

namespace FeatureManagement.Database.Abstractions;

public class FeatureStoreWithCircularReference : IFeatureStore
{
    private readonly IReadOnlyCollection<Feature> _features;

    public FeatureStoreWithCircularReference()
    {
        var firstFeature = new Feature
        {
            Name = FirstFeature,
            RequirementType = Microsoft.FeatureManagement.RequirementType.All,
        };
        firstFeature.Settings =
        [
            new FeatureSettings
            {
                FeatureId = firstFeature.Id,
                Feature = firstFeature,
                FilterType = FeatureFilterType.TimeWindow,
                Parameters =
                    """{"Start": "Mon, 01 May 2023 13:59:59 GMT", "End": "Sat, 01 July 2023 00:00:00 GMT"}"""
            }
        ];
        var secondFeature = new Feature
        {
            Name = SecondFeature,
            RequirementType = Microsoft.FeatureManagement.RequirementType.All,
        };
        secondFeature.Settings =
        [
            new FeatureSettings
            {
                FeatureId = secondFeature.Id,
                Feature = secondFeature,
                FilterType = FeatureFilterType.TimeWindow,
                Parameters =
                    """{"Start": "Mon, 01 May 2023 13:59:59 GMT", "End": "Sat, 01 July 2023 00:00:00 GMT"}"""
            }
        ];
        _features =
        [
            firstFeature,
            secondFeature
        ];
    }

    public Task<Feature> GetFeatureAsync([NotNull] string featureName)
    {
        return Task.FromResult(_features.SingleOrDefault(x => x.Name == featureName));
    }

    public Task<IReadOnlyCollection<Feature>> GetFeaturesAsync()
    {
        return Task.FromResult(_features);
    }
}