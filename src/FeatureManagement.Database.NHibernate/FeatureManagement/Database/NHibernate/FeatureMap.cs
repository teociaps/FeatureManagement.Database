// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FluentNHibernate.Mapping;

namespace FeatureManagement.Database.NHibernate;

/// <summary>
/// Defines the mapping for the <see cref="Feature"/> class to the database table.
/// </summary>
public class FeatureMap : ClassMap<Feature>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureMap"/> class.
    /// Configures the mapping for the <see cref="Feature"/> class.
    /// </summary>
    public FeatureMap()
    {
        Table($"{nameof(Feature)}s");

        Id(x => x.Id)
            .GeneratedBy.GuidComb()
            .Column(nameof(Feature.Id));

        Map(x => x.Name)
            .Column(nameof(Feature.Name))
            .Unique()
            .Not.Nullable();

        Map(x => x.RequirementType)
            .Column(nameof(Feature.RequirementType))
            .Not.Nullable();

        HasMany(x => x.Settings)
            .KeyColumn(nameof(FeatureSettings.FeatureId))
            //.Inverse()
            .Cascade.All()
            .LazyLoad();
    }
}