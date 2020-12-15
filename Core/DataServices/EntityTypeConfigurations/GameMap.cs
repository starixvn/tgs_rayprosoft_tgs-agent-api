using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class GameMap : BaseConfiguration, IEntityTypeConfiguration<Game>
    {
        void IEntityTypeConfiguration<Game>.Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("Games", "dbo");
            BaseConfigureProperties<Game, int>(builder);

            builder.Property(e => e.Id)
                .ValueGeneratedNever();

            builder.Property(e => e.Code)
                .HasColumnType(ColumnTypes.Nvarchar(128))
                .IsRequired();

            builder.Property(e => e.Status)
                .HasColumnType(ColumnTypes.TinyInt)
                .IsRequired();

            builder.Property(c => c.ExternalId)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.Property(e => e.Description)
                .HasColumnType(ColumnTypes.Nvarchar(128));

            builder.Property(e => e.EndpointPath)
                .HasColumnType(ColumnTypes.Nvarchar(2048));

            builder.Property(e => e.RuleUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(e => e.Type)
                .HasColumnType(ColumnTypes.TinyInt)
                .IsRequired();

            builder.Property(e => e.PlatformType)
                .HasColumnType(ColumnTypes.TinyInt)
                .IsRequired();

            builder.Property(e => e.SubGame)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.HasTurnoverOnTieBet)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.Html5)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.AvailableCurrencies)
                .HasColumnType(ColumnTypes.Nvarchar(1024))
                .IsRequired()
                .HasDefaultValue(string.Empty);

            builder.HasOne(e => e.GameProvider)
                .WithMany(e => e.Games)
                .HasForeignKey(e => e.GameProviderId);

            builder.HasIndex(e => e.GameProviderId)
                .IncludeProperties(e => new
                {
                    e.Id,
                    e.Code,
                    e.Name,
                    e.ExternalId,
                    e.PlatformType,
                    e.Type,
                    e.Status,
                    e.AvailableCurrencies,
                    e.EndpointPath,
                    e.SubGame,
                    e.HasTurnoverOnTieBet,
                    e.RuleUrl,
                    e.Description
                });
        }
    }
}