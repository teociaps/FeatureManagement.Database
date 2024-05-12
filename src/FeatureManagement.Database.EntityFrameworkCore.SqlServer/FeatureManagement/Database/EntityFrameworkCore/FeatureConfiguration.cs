// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeatureManagement.Database.EntityFrameworkCore;

public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.RequirementType).IsRequired();

        builder.HasMany(x => x.Settings);
        builder.Navigation(x => x.Settings).AutoInclude();
    }
}
