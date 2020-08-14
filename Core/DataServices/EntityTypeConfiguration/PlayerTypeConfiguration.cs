using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
{
    public class PlayerTypeConfiguration : IEntityTypeConfiguration<Player>
    {
        void IEntityTypeConfiguration<Player>.Configure(EntityTypeBuilder<Player> builder)
        {
            builder.ToTable("Players", "dbo");
            builder.HasKey(bc => bc.PlayerCode);

            builder.Property(c => c.PlayerCode)
                .HasColumnType("varchar(16)")
                .IsRequired()
                .IsUnicode();
        }
    }
}
