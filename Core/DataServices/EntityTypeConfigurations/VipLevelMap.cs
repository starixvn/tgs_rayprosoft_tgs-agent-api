using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class VipLevelMap : IEntityTypeConfiguration<VipLevel>
    {
        void IEntityTypeConfiguration<VipLevel>.Configure(EntityTypeBuilder<VipLevel> builder)
        {
            builder.ToTable("VipLevels", "dbo");
            builder.HasKey(bc => bc.Id);

            builder.Property(c => c.Code)
                .HasColumnType(ColumnTypes.Varchar(16))
                .IsRequired();

            builder.Property(c => c.Name)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();
        }
    }
}