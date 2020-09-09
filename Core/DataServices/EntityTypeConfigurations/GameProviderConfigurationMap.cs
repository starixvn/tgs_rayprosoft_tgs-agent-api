using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class GameProviderConfigurationMap : IEntityTypeConfiguration<GameProviderConfiguration>
    {
        void IEntityTypeConfiguration<GameProviderConfiguration>.Configure(EntityTypeBuilder<GameProviderConfiguration> builder)
        {
            builder.ToTable("GameProviderConfigurations", "dbo");

            builder.HasKey(p => p.Id)
                .IsClustered(false);

            builder.Property(p => p.Name)
                .HasColumnType(ColumnTypes.Nvarchar(255));

            builder.HasOne(p => p.GameProvider)
                .WithMany(p => p.GameProviderConfigurations)
                .HasForeignKey(p => p.GameProviderId);

            builder.Property(c => c.Description)
                .HasColumnType(ColumnTypes.Nvarchar(256));

            builder.Property(c => c.SecretKey)
                .HasColumnType(ColumnTypes.Varchar(64))
                .IsRequired();

            builder.Property(c => c.DesktopEndpoint)
                .HasColumnType(ColumnTypes.Nvarchar(1024))
                .IsRequired();

            builder.Property(c => c.MobileEndpoint)
                .HasColumnType(ColumnTypes.Nvarchar(1024))
                .IsRequired();

            builder.Property(c => c.ConfigurationData)
                .HasColumnType(ColumnTypes.Nvarchar(4000))
                .IsRequired();

            builder.Property(c => c.IsDefault)
                .IsRequired();

            builder.HasMany(c => c.LicenseeGameProviderConfigurations)
                .WithOne(c => c.GameProviderConfiguration)
                .HasForeignKey(c => c.GameProviderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}