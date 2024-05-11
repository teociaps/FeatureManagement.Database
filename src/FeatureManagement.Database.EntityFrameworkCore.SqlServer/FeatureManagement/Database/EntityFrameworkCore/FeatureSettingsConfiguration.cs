using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeatureManagement.Database.EntityFrameworkCore;

public class FeatureSettingsConfiguration : IEntityTypeConfiguration<FeatureSettings>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<FeatureSettings> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Feature);
        //TODO: add more config
    }
}
