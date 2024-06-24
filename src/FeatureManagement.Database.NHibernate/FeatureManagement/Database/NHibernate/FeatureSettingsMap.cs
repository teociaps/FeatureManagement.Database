// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FluentNHibernate.Mapping;

namespace FeatureManagement.Database.NHibernate;

/// <summary>
/// Defines the mapping for the <see cref="FeatureSettings"/> class to the database table.
/// </summary>
public class FeatureSettingsMap : ClassMap<FeatureSettings>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureSettingsMap"/> class.
    /// Configures the mapping for the <see cref="FeatureSettings"/> class.
    /// </summary>
    public FeatureSettingsMap()
    {
        Table(nameof(FeatureSettings));

        Id(x => x.Id)
            .GeneratedBy.GuidComb()
            .Column(nameof(FeatureSettings.Id));

        Map(x => x.CustomFilterTypeName)
            .Column(nameof(FeatureSettings.CustomFilterTypeName));

        Map(x => x.FilterType)
            .Column(nameof(FeatureSettings.FilterType))
            .Not.Nullable();

        Map(x => x.Parameters)
            .Column(nameof(FeatureSettings.Parameters))
            .Not.Nullable();

        References(x => x.Feature)
            .Column(nameof(FeatureSettings.FeatureId))
            //.Not.Nullable()
            .LazyLoad();
    }
}