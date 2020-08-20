using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    public class PlayerTypeConfiguration : IEntityTypeConfiguration<Player>
    {
        void IEntityTypeConfiguration<Player>.Configure(EntityTypeBuilder<Player> builder)
        {
            builder.ToTable("Players", "dbo");
            builder.HasKey(bc => bc.PlayerCode);

            builder.Property(c => c.PlayerCode)
                .HasColumnType("varchar(16)")
                .IsRequired();

            builder.HasMany(p => p.Wallets)
                .WithOne(p => p.Player)
                .HasForeignKey(p => p.PlayerCode);

            builder.Property(c => c.Username)
                .HasColumnType("nvarchar(32)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.ExternalId)
                .HasColumnType("nvarchar(128)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.IpAddress)
                .HasColumnType("nvarchar(64)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.CultureCode)
                .HasColumnType("nvarchar(10)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.BetLimitGroupId)
                .HasColumnType("int")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.DefaultWalletCurrencyCode)
                .HasColumnType("nvarchar(16)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.CreatedOn)
                .HasColumnType("datetimeoffset")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.LastActiveOn)
                .HasColumnType("datetimeoffset")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.LastLoginUrl)
                .HasColumnType("nvarchar(1024)")
                .IsUnicode();

            builder.Property(c => c.LastCashierUrl)
                .HasColumnType("nvarchar(1024)")
                .IsUnicode();

            builder.Property(c => c.LastHelpUrl)
                .HasColumnType("nvarchar(1024)")
                .IsUnicode();

            builder.Property(c => c.LastTermsUrl)
                .HasColumnType("nvarchar(1024)")
                .IsUnicode();
        }
    }
}
