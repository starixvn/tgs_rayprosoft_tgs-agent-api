using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class MarketTypeMap : IEntityTypeConfiguration<MarketType>
    {
        void IEntityTypeConfiguration<MarketType>.Configure(EntityTypeBuilder<MarketType> builder)
        {
            builder.ToTable("MarketTypes", "dbo");
            builder.HasKey(e => e.Id);
            builder.HasAlternateKey(e => new {e.GameProviderId, e.Code});

            builder.Property(e => e.Code)
                .HasColumnType(ColumnTypes.Nvarchar(64))
                .IsRequired();

            builder.Property(e => e.ExternalId)
                .HasColumnType(ColumnTypes.Nvarchar(16))
                .IsRequired();

            builder.HasOne(e => e.GameProvider)
                .WithMany(e => e.MarketTypes)
                .HasForeignKey(e => e.Id);
        }
    }
}
