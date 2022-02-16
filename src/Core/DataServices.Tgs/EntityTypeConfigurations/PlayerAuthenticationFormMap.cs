using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PlayerAuthenticationFormMap : IEntityTypeConfiguration<PlayerAuthenticationForm>
    {
        void IEntityTypeConfiguration<PlayerAuthenticationForm>.Configure(EntityTypeBuilder<PlayerAuthenticationForm> builder)
        {
            builder.ToTable("player_authentication_forms");

            builder.HasKey(paf => paf.Id)
                .HasName("pk_player_authentication_forms");

            builder.Property(paf => paf.Id)
                .IsRequired();

            builder.Property(paf => paf.PlayerCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(paf => paf.BrandId)
                .IsRequired();

            builder.Property(paf => paf.CreatedOn)
                .IsRequired();

            builder.Property(paf => paf.Remark);

            builder.Property(paf => paf.Data)
                .HasColumnType(ColumnTypes.Jsonb)
                .IsRequired();

            builder.Property(paf => paf.Status)
                .IsRequired()
                .HasDefaultValue(0);
        }
    }
}
