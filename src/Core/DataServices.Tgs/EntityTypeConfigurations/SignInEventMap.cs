using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public class SignInEventMap : IEntityTypeConfiguration<SignInEvent>
    {
        public void Configure(EntityTypeBuilder<SignInEvent> builder)
        {
            builder.ToTable("sign_in_events");

            builder.Property(s => s.Id)
                .IsRequired();

            builder.HasKey(s => s.Id)
                .HasName("pk_sign_in_events");

            builder.Property(s => s.BrandId)
                .IsRequired();
            builder.Property(s => s.PlayerCode)
                .IsRequired();
            builder.Property(s => s.Date)
                .IsRequired()
                .HasColumnType("date");
            builder.Property(s => s.CreatedOn)
                .IsRequired()
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("Now()");

            builder.HasOne<Brand>()
                .WithMany()
                .HasForeignKey(s => s.BrandId)
                .HasConstraintName("fk_sign_in_events_brands_brand_id")
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<Player>()
                .WithMany()
                .HasForeignKey(s => s.PlayerCode)
                .HasConstraintName("fk_sign_in_events_players_brand_id")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
