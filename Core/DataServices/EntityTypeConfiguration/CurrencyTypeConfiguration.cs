using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
{
    public class CurrencyTypeConfiguration : IEntityTypeConfiguration<Currency>
    {
        void IEntityTypeConfiguration<Currency>.Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.ToTable("Currecies", "dbo");
            builder.HasKey(p => p.Code);
        }
    }
}
