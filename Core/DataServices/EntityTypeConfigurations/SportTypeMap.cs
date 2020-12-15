using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class SportTypeMap : IEntityTypeConfiguration<SportType>
    {
        void IEntityTypeConfiguration<SportType>.Configure(EntityTypeBuilder<SportType> builder)
        {
            builder.ToTable("SportTypes", "dbo");
            builder.HasKey(e => e.Id);
            builder.HasAlternateKey(e => new {e.GameProviderId, Name = e.Code});

            builder.Property(e => e.Code)
                .HasColumnType(ColumnTypes.Nvarchar(64))
                .IsRequired();

            builder.Property(e => e.ExternalId)
                .HasColumnType(ColumnTypes.Nvarchar(16))
                .IsRequired();

            builder.HasOne(e => e.GameProvider)
                .WithMany(e => e.SportTypes)
                .HasForeignKey(e => e.Id);
        }
    }
}
