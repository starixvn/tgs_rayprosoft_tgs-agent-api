using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Bet.Models;

namespace TGS.Core.DataServices.Bet.EntityTypeConfigurations
{
    internal class RoundDetailMap : IEntityTypeConfiguration<RoundDetail>
    {
        void IEntityTypeConfiguration<RoundDetail>.Configure(EntityTypeBuilder<RoundDetail> builder)
        {
            builder.ToTable("RoundDetails", "dbo");
            builder.HasKey(bc => bc.RoundId).IsClustered(true);

            builder.Property(c => c.AdditionalData)
                .HasColumnType(ColumnTypes.NvarcharMax)
                .IsRequired();
        }
    }
}
