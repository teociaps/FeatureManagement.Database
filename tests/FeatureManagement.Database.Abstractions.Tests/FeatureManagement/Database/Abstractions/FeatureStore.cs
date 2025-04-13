// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;
using static FeatureManagement.Database.Abstractions.Features;

namespace FeatureManagement.Database.Abstractions;

public class FeatureStore : IFeatureStore
{
    private readonly List<Feature> _features;

    internal static readonly Guid _firstFeatureId = Guid.NewGuid();
    internal static readonly Guid _secondFeatureId = Guid.NewGuid();

    public FeatureStore()
    {

        var firstFeature = new Feature
        {
            Id = _firstFeatureId,
            Name = FirstFeature,
            RequirementType = Microsoft.FeatureManagement.RequirementType.All,
            Settings = []
        };

        var secondFeature = new Feature
        {
            Id = _secondFeatureId,
            Name = SecondFeature,
            RequirementType = Microsoft.FeatureManagement.RequirementType.All,
            Settings = []
        };

        firstFeature.Settings.Add(new FeatureSettings
        {
            Id = Guid.NewGuid(),
            FilterType = FeatureFilterType.TimeWindow,
            Parameters = """{"Start": "Mon, 01 May 2023 13:59:59 GMT", "End": "Sat, 01 July 2023 00:00:00 GMT"}""",
            FeatureId = _firstFeatureId
        });

        secondFeature.Settings.Add(new FeatureSettings
        {
            Id = Guid.NewGuid(),
            FilterType = FeatureFilterType.TimeWindow,
            Parameters = """{"Start": "Mon, 01 May 2023 13:59:59 GMT", "End": "Sat, 01 July 2023 00:00:00 GMT"}""",
            FeatureId = _secondFeatureId
        });

        _features = [firstFeature, secondFeature];
    }

    public Task<Feature> GetFeatureAsync([NotNull] string featureName)
    {
        return Task.FromResult(_features.SingleOrDefault(x => x.Name == featureName));
    }

    public Task<Feature> GetFeatureAsync(Guid featureId)
    {
        var feature = _features.SingleOrDefault(x => x.Id == featureId);
        return Task.FromResult(feature);
    }

    public Task<IReadOnlyCollection<Feature>> GetFeaturesAsync()
    {
        return Task.FromResult<IReadOnlyCollection<Feature>>(_features);
    }

    public Task<Feature> CreateFeatureAsync(Feature feature)
    {
        feature.Id = Guid.NewGuid();
        _features.Add(feature);
        return Task.FromResult(feature);
    }

    public Task<Feature> UpdateFeatureAsync(Feature feature)
    {
        var existingFeature = _features.SingleOrDefault(f => f.Id == feature.Id)
            ?? throw new KeyNotFoundException("Feature not found.");

        existingFeature.Name = feature.Name;
        existingFeature.RequirementType = feature.RequirementType;
        existingFeature.Settings = feature.Settings;

        return Task.FromResult(existingFeature);
    }

    public Task DeleteFeatureAsync(Guid featureId)
    {
        var feature = _features.SingleOrDefault(f => f.Id == featureId)
            ?? throw new KeyNotFoundException("Feature not found.");

        _features.Remove(feature);
        return Task.CompletedTask;
    }

    public Task<FeatureSettings> GetFeatureSettingAsync(Guid featureSettingId)
    {
        var featureSetting = _features
            .SelectMany(f => f.Settings)
            .SingleOrDefault(s => s.Id == featureSettingId);

        return Task.FromResult(featureSetting);
    }

    public Task<FeatureSettings> CreateFeatureSettingAsync(FeatureSettings featureSetting)
    {
        featureSetting.Id = Guid.NewGuid();
        var feature = _features.SingleOrDefault(f => f.Id == featureSetting.FeatureId)
            ?? throw new KeyNotFoundException("Feature not found.");

        feature.Settings.Add(featureSetting);
        return Task.FromResult(featureSetting);
    }

    public Task<FeatureSettings> UpdateFeatureSettingAsync(FeatureSettings featureSetting)
    {
        var feature = _features.SingleOrDefault(f => f.Id == featureSetting.FeatureId)
            ?? throw new KeyNotFoundException("Feature not found.");

        var existingSetting = feature.Settings.SingleOrDefault(s => s.Id == featureSetting.Id)
            ?? throw new KeyNotFoundException("Feature setting not found.");

        existingSetting.FilterType = featureSetting.FilterType;
        existingSetting.Parameters = featureSetting.Parameters;
        existingSetting.CustomFilterTypeName = featureSetting.CustomFilterTypeName;

        return Task.FromResult(existingSetting);
    }

    public Task DeleteFeatureSettingAsync(Guid featureSettingId)
    {
        var feature = _features.SingleOrDefault(f => f.Settings.Any(s => s.Id == featureSettingId))
            ?? throw new KeyNotFoundException("Feature not found.");

        var featureSetting = feature.Settings.SingleOrDefault(s => s.Id == featureSettingId)
            ?? throw new KeyNotFoundException("Feature setting not found.");

        feature.Settings.Remove(featureSetting);
        return Task.CompletedTask;
    }
}

// Just a fake feature store for testing purposes
public class FeatureNonStore
{
    public FeatureNonStore()
    {
    }
}