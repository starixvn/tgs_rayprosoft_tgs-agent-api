using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BrandLiveChatMap : IEntityTypeConfiguration<BrandLiveChat>
    {
        void IEntityTypeConfiguration<BrandLiveChat>.Configure(EntityTypeBuilder<BrandLiveChat> builder)
        {
            builder.ToTable("brand_live_chats");

            builder.HasKey(e => e.BrandId);

            builder.HasOne(e => e.Brand)
                .WithOne()
                .HasForeignKey<BrandLiveChat>(e => e.BrandId)
                .HasConstraintName("fk_brand_live_chats_brands_brand_id");

            builder.Property(e => e.ChatName)
                .HasMaxLength(128);

            builder.Property(e => e.ChatSetting)
                .HasColumnType(ColumnTypes.Jsonb);
        }
    }
}