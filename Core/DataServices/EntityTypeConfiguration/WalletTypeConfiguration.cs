using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
{
    public class WalletTypeConfiguration : IEntityTypeConfiguration<Wallet>
    {
        void IEntityTypeConfiguration<Wallet>.Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallets", "dbo");
            builder.HasKey(bc => bc.Id);
        }
    }
}
