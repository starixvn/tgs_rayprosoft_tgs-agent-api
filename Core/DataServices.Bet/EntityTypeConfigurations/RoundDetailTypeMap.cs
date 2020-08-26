using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Bet.Models;

namespace TGS.Core.DataServices.Bet.EntityTypeConfigurations
{
    internal class RoundDetailTypeMap : IEntityTypeConfiguration<RoundDetail>
    {
        void IEntityTypeConfiguration<RoundDetail>.Configure(EntityTypeBuilder<RoundDetail> builder)
        {
            builder.ToTable("RoundDetails", "dbo");
            builder.HasKey(bc => bc.Id).IsClustered(false);

            builder.Property(c => c.AdditionalData)
                .HasColumnType("nvarchar(max)")
                .IsRequired();
        }
    }
}
