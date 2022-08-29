using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class PlayerBankInfoTagMap : IEntityTypeConfiguration<PlayerBankInfoTag>
{
    public void Configure(EntityTypeBuilder<PlayerBankInfoTag> builder)
    {
        builder.ToTable("player_bank_info_tags");

        builder.HasKey(bt => new { bt.BrandId, bt.PlayerCode, bt.BankInfoId, bt.TagType });

        builder.Property(bt => bt.Tag)
            .IsRequired();
    }
}
