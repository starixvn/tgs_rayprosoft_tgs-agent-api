using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
{
    public class ResellerLanguageTypeConfiguration : IEntityTypeConfiguration<ResellerLanguage>
    {
        void IEntityTypeConfiguration<ResellerLanguage>.Configure(EntityTypeBuilder<ResellerLanguage> builder)
        {
            builder.ToTable("ResellerLanguages", "dbo");
            builder.HasKey(bc => new { bc.ResellerId, bc.CultureCode });
            builder
                .HasOne(bc => bc.Reseller)
                .WithMany(c => c.Languages)
                .HasForeignKey(bc => bc.ResellerId);
            builder
                .HasOne(bc => bc.Language)
                .WithMany(c => c.ResellerLanguages)
                .HasForeignKey(bc => bc.CultureCode);
        }
    }
}
