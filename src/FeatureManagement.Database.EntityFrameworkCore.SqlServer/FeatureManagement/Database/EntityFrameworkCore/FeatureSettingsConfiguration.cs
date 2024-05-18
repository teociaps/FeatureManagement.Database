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
        builder.HasKey(fs => fs.Id);
        builder.Property(fs => fs.Id).ValueGeneratedOnAdd();

        builder.Property(fs => fs.FilterType).IsRequired();

        builder.Property(fs => fs.Parameters).IsRequired();
    }
}
