using FileService.DataService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileService.DataService.EntityTypeConfigurations
{
    public sealed class StationLetterFileMap : IEntityTypeConfiguration<StationLetterFile>
    {
        void IEntityTypeConfiguration<StationLetterFile>.Configure(EntityTypeBuilder<StationLetterFile> builder)
        {
            builder.ToTable("StationLetterFiles", "dbo");

            builder.HasKey(p => p.StationLetterId);

            builder.HasMany(c => c.Files)
                .WithOne()
                .HasForeignKey(c => c.Id);
        }
    }
}