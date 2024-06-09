// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeatureManagement.Database.EntityFrameworkCore;

/// <summary>
/// Configuration for <see cref="Feature"/> entity.
/// </summary>
public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).ValueGeneratedOnAdd();

        builder.Property(f => f.Name).IsRequired();
        builder.HasIndex(f => f.Name).IsUnique();

        builder.Property(f => f.RequirementType).IsRequired();

        builder.HasMany(f => f.Settings)
               .WithOne(fs => fs.Feature)
               .HasForeignKey(f => f.FeatureId);
        builder.Navigation(f => f.Settings).AutoInclude();
    }
}