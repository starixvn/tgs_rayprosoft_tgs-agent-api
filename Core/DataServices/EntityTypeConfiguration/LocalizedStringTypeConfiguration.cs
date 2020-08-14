using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
{
    public class LocalizedStringTypeConfiguration : IEntityTypeConfiguration<LocalizedString>
    {
        void IEntityTypeConfiguration<LocalizedString>.Configure(EntityTypeBuilder<LocalizedString> builder)
        {
            builder.ToTable("LocalizedStrings", "dbo");
            builder.HasKey(bc => new { bc.Context, bc.CultureCode, bc.Code });
        }
    }
}
