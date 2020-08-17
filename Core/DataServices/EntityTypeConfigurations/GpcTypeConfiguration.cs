using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    public class GpcTypeConfiguration : BaseConfiguration, IEntityTypeConfiguration<GameProviderConfiguration>
    {
        void IEntityTypeConfiguration<GameProviderConfiguration>.Configure(EntityTypeBuilder<GameProviderConfiguration> builder)
        {
            builder.ToTable("GameProviderConfigurations", "dbo");
            BaseConfigureProperties(builder);

            builder.HasOne(p => p.GameProvider)
                .WithMany(p => p.GameProviderConfigurations)
                .HasForeignKey(p => p.GameProviderId);

            builder.Property(c => c.Description)
                .HasColumnType("nvarchar(256)");

            builder.Property(c => c.SecretKey)
                .HasColumnType("varchar(64)")
                .IsRequired();

            builder.Property(c => c.DesktopEndpoint)
                .HasColumnType("nvarchar(1024)")
                .IsRequired();

            builder.Property(c => c.MobileEndpoint)
                .HasColumnType("nvarchar(1024)")
                .IsRequired();
            builder.Property(c => c.ConfigurationData)
                .HasColumnType("nvarchar(4000)")
                .IsRequired();
        }
    }
}
