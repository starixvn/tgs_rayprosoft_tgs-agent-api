using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    public class GameProviderLanguageTypeConfiguration : IEntityTypeConfiguration<GameProviderLanguage>
    {
        void IEntityTypeConfiguration<GameProviderLanguage>.Configure(EntityTypeBuilder<GameProviderLanguage> builder)
        {
            builder.ToTable("GameProviderLanguages", "dbo");
            builder.HasKey(bc => new { bc.GameProviderId, bc.CultureCode });
            builder
                .HasOne(bc => bc.GameProvider)
                .WithMany(c => c.Languages)
                .HasForeignKey(bc => bc.GameProviderId);
            builder
                .HasOne(bc => bc.Language)
                .WithMany(c => c.GameProviderLanguages)
                .HasForeignKey(bc => bc.CultureCode);
        }
    }
}
