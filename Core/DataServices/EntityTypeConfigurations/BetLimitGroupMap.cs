using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class BetLimitGroupMap : IEntityTypeConfiguration<BetLimitGroup>
    {
        void IEntityTypeConfiguration<BetLimitGroup>.Configure(EntityTypeBuilder<BetLimitGroup> builder)
        {
            builder.ToTable("BetLimitGroups", "dbo");
            builder.HasKey(b => b.Id)
                .IsClustered(false);

            builder.Property(b => b.Code)
                .HasColumnType("nvarchar(20)")
                .IsRequired();

            builder.Property(b => b.Name)
                .HasColumnType("nvarchar(255)")
                .IsRequired();

            builder.Property(b => b.Description)
                .HasColumnType("nvarchar(512)");

            builder.Property(b => b.CreatedOn)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            builder.Property(b => b.UpdatedOn)
                .HasColumnType("datetimeoffset")
                .IsRequired();

             builder.Property(b => b.CreatedByUsername)
                .HasColumnType("nvarchar(255)")
                .IsRequired();

            builder.Property(b => b.UpdatedByUsername)
                .HasColumnType("nvarchar(255)")
                .IsRequired();

            builder.HasMany(b => b.Players)
                .WithOne(b => b.BetLimitGroup)
                .HasForeignKey(b => b.BetLimitGroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
