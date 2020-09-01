using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class GameProviderLanguageMap : IEntityTypeConfiguration<GameProviderLanguage>
    {
        void IEntityTypeConfiguration<GameProviderLanguage>.Configure(EntityTypeBuilder<GameProviderLanguage> builder)
        {
            builder.ToTable("GameProviderLanguages", "dbo");
            builder.HasKey(gpl => new {gpl.GameProviderId, gpl.CultureCode});

            builder
                .HasOne(gpl => gpl.GameProvider)
                .WithMany(gp => gp.Languages)
                .HasForeignKey(gpl => gpl.GameProviderId);
            
            builder
                .HasOne(gpl => gpl.Language)
                .WithMany(l => l.GameProviderLanguages)
                .HasForeignKey(gpl => gpl.CultureCode);

            builder.Property(gpl => gpl.CultureCode)
                .HasColumnType(ColumnTypes.Nvarchar(6));

            builder.Property(gpl => gpl.GameProviderCultureCode)
                .HasColumnType(ColumnTypes.Nvarchar(6))
                .IsRequired();
        }
    }
}
