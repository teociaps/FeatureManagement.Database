// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using ConsoleApp.FeatureFilters;
using FeatureManagement.Database;
using System.Diagnostics.CodeAnalysis;
using static ConsoleApp.Features;

namespace ConsoleApp;

public class FeatureStore : IFeatureStore
{
    private readonly List<Feature> _features;

    public FeatureStore()
    {
        _features =
        [
            new Feature
            {
                Name = Beta,
                Settings =
                [
                    new FeatureSettings
                    {
                        CustomFilterTypeName = UsernameFilter.Name,
                        FilterType = FeatureFilterType.Custom,
                        Parameters = """{ "AllowedUsernames": [ "Matthew" ] }"""
                    }
                ]
            }
        ];
    }

    public Task<Feature?> GetFeatureAsync([NotNull] string featureName)
    {
        return Task.FromResult(_features.SingleOrDefault(x => x.Name == featureName));
    }

    public Task<Feature?> GetFeatureAsync(Guid featureId)
    {
        return Task.FromResult(_features.SingleOrDefault(x => x.Id == featureId));
    }

    public Task<IReadOnlyCollection<Feature>> GetFeaturesAsync()
    {
        return Task.FromResult((IReadOnlyCollection<Feature>)_features);
    }

    public Task<Feature> CreateFeatureAsync([NotNull] Feature feature)
    {
        ArgumentNullException.ThrowIfNull(feature);

        if (_features.Any(f => f.Name == feature.Name))
            throw new InvalidOperationException($"A feature with the name '{feature.Name}' already exists.");

        _features.Add(feature);
        return Task.FromResult(feature);
    }

    public Task<Feature> UpdateFeatureAsync([NotNull] Feature feature)
    {
        ArgumentNullException.ThrowIfNull(feature);

        var existingFeature = _features.SingleOrDefault(f => f.Name == feature.Name)
            ?? throw new KeyNotFoundException($"Feature with the name '{feature.Name}' not found.");

        _features.Remove(existingFeature);
        _features.Add(feature);

        return Task.FromResult(feature);
    }

    public Task DeleteFeatureAsync(Guid featureId)
    {
        var feature = _features.SingleOrDefault(f => f.Id == featureId)
            ?? throw new KeyNotFoundException($"Feature with Id '{featureId}' not found.");

        _features.Remove(feature);

        return Task.CompletedTask;
    }

    public Task<FeatureSettings> GetFeatureSettingAsync(Guid featureSettingId)
    {
        var featureSetting = _features
            .SelectMany(f => f.Settings)
            .SingleOrDefault(s => s.Id == featureSettingId)
            ?? throw new KeyNotFoundException($"Feature setting with Id '{featureSettingId}' not found.");

        return Task.FromResult(featureSetting);
    }

    public Task<FeatureSettings> CreateFeatureSettingAsync([NotNull] FeatureSettings featureSetting)
    {
        ArgumentNullException.ThrowIfNull(featureSetting);

        var feature = _features.SingleOrDefault(f => f.Id == featureSetting.FeatureId)
            ?? throw new KeyNotFoundException($"Feature with Id '{featureSetting.FeatureId}' not found.");

        if (feature.Settings.Any(s => s.Id == featureSetting.Id))
            throw new InvalidOperationException($"A feature setting with the Id '{featureSetting.Id}' already exists.");

        feature.Settings.Add(featureSetting);
        return Task.FromResult(featureSetting);
    }

    public Task<FeatureSettings> UpdateFeatureSettingAsync([NotNull] FeatureSettings featureSetting)
    {
        ArgumentNullException.ThrowIfNull(featureSetting);

        var feature = _features.SingleOrDefault(f => f.Id == featureSetting.FeatureId)
            ?? throw new KeyNotFoundException($"Feature with Id '{featureSetting.FeatureId}' not found.");

        var existingSetting = feature.Settings.SingleOrDefault(s => s.Id == featureSetting.Id)
            ?? throw new KeyNotFoundException($"Feature setting with Id '{featureSetting.Id}' not found.");

        feature.Settings.Remove(existingSetting);
        feature.Settings.Add(featureSetting);

        return Task.FromResult(featureSetting);
    }

    public Task DeleteFeatureSettingAsync(Guid featureSettingId)
    {
        var feature = _features.SingleOrDefault(f => f.Settings.Any(s => s.Id == featureSettingId))
            ?? throw new KeyNotFoundException($"Feature setting with Id '{featureSettingId}' not found.");

        var featureSetting = feature.Settings.Single(s => s.Id == featureSettingId);
        feature.Settings.Remove(featureSetting);

        return Task.CompletedTask;
    }
}