// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeatureManagement.Database.EntityFrameworkCore;

/// <summary>
/// Configuration for <see cref="FeatureSettings"/> entity.
/// </summary>
public class FeatureSettingsConfiguration : IEntityTypeConfiguration<FeatureSettings>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<FeatureSettings> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.FilterType).IsRequired();

        builder.Property(x => x.Parameters).IsRequired();

        builder.HasOne(x => x.Feature);
        builder.Navigation(x => x.Feature).AutoInclude();
    }
}
