using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
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

            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();

            builder.Property(b => b.Code)
                .HasColumnType(ColumnTypes.Nvarchar(20))
                .IsRequired();

            builder.Property(b => b.Name)
                .HasColumnType(ColumnTypes.Nvarchar(255))
                .IsRequired();

            builder.Property(b => b.Description)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(b => b.CreatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(b => b.UpdatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

             builder.Property(b => b.CreatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.Property(b => b.UpdatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.HasMany(b => b.Players)
                .WithOne(b => b.BetLimitGroup)
                .HasForeignKey(b => b.BetLimitGroupId);
        }
    }
}
