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
        Table("FeatureSettings");

        Id(x => x.Id)
            .GeneratedBy.GuidComb()
            .Column("Id");

        Map(x => x.CustomFilterTypeName)
            .Column("CustomFilterTypeName");

        Map(x => x.FilterType)
            .Column("FilterType")
            .Not.Nullable();

        Map(x => x.Parameters)
            .Column("Parameters")
            .Not.Nullable();

        References(x => x.Feature)
            .Column("FeatureId")
            //.Not.Nullable()
            .LazyLoad();
    }
}