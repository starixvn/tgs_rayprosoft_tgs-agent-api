using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PlayerBucketHistoryMap : IEntityTypeConfiguration<PlayerBucketHistory>
    {
        void IEntityTypeConfiguration<PlayerBucketHistory>.Configure(EntityTypeBuilder<PlayerBucketHistory> builder)
        {
            builder.ToTable("player_bucket_histories");

            builder.HasKey(c => c.Id)
                .HasName("pk_player_bucket_histories");

            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.From)
                .IsRequired();

            builder.Property(c => c.To)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.HasIndex(c => c.PlayerCode)
                .HasDatabaseName("ix_player_bucket_histories_player_code");
        }
    }
}