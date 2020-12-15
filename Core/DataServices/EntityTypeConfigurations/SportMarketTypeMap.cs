using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class SportMarketTypeMap : IEntityTypeConfiguration<SportMarketType>
    {
        void IEntityTypeConfiguration<SportMarketType>.Configure(EntityTypeBuilder<SportMarketType> builder)
        {
            builder.ToTable("SportMarketTypes", "dbo");
            
            builder.HasKey(e => e.Id);
            builder.HasAlternateKey(e => new {e.SportTypeId, e.MarketTypeId});

            builder.HasOne(e => e.SportType)
                .WithMany(e => e.MarketTypes)
                .HasForeignKey(e => e.SportTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.MarketType)
                .WithMany(e => e.SportTypes)
                .HasForeignKey(e => e.MarketTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
