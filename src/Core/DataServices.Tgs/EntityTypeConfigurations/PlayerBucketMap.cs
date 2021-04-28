using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PlayerBucketMap : IEntityTypeConfiguration<PlayerBucket>
    {
        void IEntityTypeConfiguration<PlayerBucket>.Configure(EntityTypeBuilder<PlayerBucket> builder)
        {
            builder.ToTable("player_buckets");

            builder.HasKey(c => c.Id)
                .HasName("pk_player_buckets");

            builder.Property(c => c.BucketAmount)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(c => c.BucketType)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();
        }
    }
}