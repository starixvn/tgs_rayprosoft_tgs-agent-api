using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Migrations;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class RoundDetailMap : IEntityTypeConfiguration<RoundDetail>
    {
        void IEntityTypeConfiguration<RoundDetail>.Configure(EntityTypeBuilder<RoundDetail> builder)
        {
            builder.ToTable("round_details", Consts.BetSchema);

            builder.HasKey(c => c.RoundId)
                .HasName("pk_round_details");

            builder.Property(c => c.AdditionalData)
                .IsRequired();

            builder.HasOne(c => c.Round)
                .WithOne(c => c.RoundDetail)
                .HasForeignKey<RoundDetail>(c => c.RoundId)
                .HasPrincipalKey<Round>(c => c.Id)
                .HasConstraintName("fk_round_details_rounds_round_id");
        }
    }
}