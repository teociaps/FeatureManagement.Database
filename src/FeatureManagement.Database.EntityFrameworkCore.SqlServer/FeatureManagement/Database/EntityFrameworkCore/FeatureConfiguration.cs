using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeatureManagement.Database.EntityFrameworkCore;

public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Settings);
        builder.HasIndex(x => x.Name).IsUnique();
        //TODO: add more config??
    }
}
