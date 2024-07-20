// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

namespace FeatureManagement.Database.Abstractions.Tests;

public class FeatureSettingsTests
{
    [Fact]
    public void ConfigureFeatureSettings()
    {
        var featureSettings = new FeatureSettings
        {
            FilterType = FeatureFilterType.AlwaysOn
        };

        Assert.Equal("AlwaysOn", featureSettings.GetFilterType());
    }

    [Fact]
    public void ConfigureFeatureSettingsWithCustomFilterType()
    {
        var featureSettings = new FeatureSettings();
        Assert.Throws<ArgumentException>(() => featureSettings.FilterType = FeatureFilterType.Custom);

        featureSettings.CustomFilterTypeName = "MyFeatureFilterType";
        featureSettings.FilterType = FeatureFilterType.Custom;
        Assert.Throws<ArgumentException>(() => featureSettings.CustomFilterTypeName = null);
        Assert.Throws<ArgumentException>(() => featureSettings.CustomFilterTypeName = "");

        Assert.Equal("MyFeatureFilterType", featureSettings.GetFilterType());
    }
}