using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BrandAllAgentGameTypeMap : IEntityTypeConfiguration<BrandAllAgentGameType>
    {
        void IEntityTypeConfiguration<BrandAllAgentGameType>.Configure(EntityTypeBuilder<BrandAllAgentGameType> builder)
        {
            builder.ToTable("brand_all_agent_game_types");

            builder.HasKey(bc => new
                {
                    BrandAllAgentId = bc.BrandId,
                    bc.GameType
                })
                .HasName("pk_brand_all_agent_game_types");

            builder.Property(c => c.GameType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();
        }
    }
}