using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
{
    public class LanguageTypeConfiguration : IEntityTypeConfiguration<Language>
    {
        void IEntityTypeConfiguration<Language>.Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("Languages", "dbo");
            builder.HasKey(p => p.LanguageCode);
        }
    }
}
