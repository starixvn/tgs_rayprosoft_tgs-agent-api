using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
{
    public class WhitelistTypeConfiguration : IEntityTypeConfiguration<Whitelist>
    {
        void IEntityTypeConfiguration<Whitelist>.Configure(EntityTypeBuilder<Whitelist> builder)
        {
            builder.ToTable("Whitelists", "dbo");
            builder.HasKey(bc => bc.Id);
        }
    }
}
