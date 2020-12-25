using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PlayerBankMap : IEntityTypeConfiguration<PlayerBank>
    {
        void IEntityTypeConfiguration<PlayerBank>.Configure(EntityTypeBuilder<PlayerBank> builder)
        {
            builder.ToTable("PlayerBanks", "dbo");
            builder.HasKey(bc => bc.Id);

            builder.Property(c => c.PlayerCode)
                .HasColumnType(ColumnTypes.Varchar(16))
                .IsRequired();

            builder.Property(c => c.RealName)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.Property(c => c.BankName)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.Property(c => c.BankAccount)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();
        }
    }
}