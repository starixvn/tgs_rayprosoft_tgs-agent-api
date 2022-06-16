using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

public class BrandAllAgentGameTypeMap : IEntityTypeConfiguration<BrandAllAgentGameType>
{
    void IEntityTypeConfiguration<BrandAllAgentGameType>.Configure(EntityTypeBuilder<BrandAllAgentGameType> builder)
    {
        builder.ToTable("brand_all_agent_game_types");

        builder.HasKey(bc => new
            {
                BrandAllAgentId = bc.BrandId,
                GameType = bc.AllAgentGameType
            })
            .HasName("pk_brand_all_agent_game_types");

        builder.Property(c => c.AllAgentGameType)
            .HasColumnType(ColumnTypes.SmallInt)
            .IsRequired();
    }
}