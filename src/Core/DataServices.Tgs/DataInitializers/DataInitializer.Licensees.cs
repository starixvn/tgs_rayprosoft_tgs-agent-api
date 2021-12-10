using System;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    public partial class DataInitializer
    {
        internal interface ILicensee
        {
            IBrand[] Brands { get; }

            string Code { get; }

            ICurrency[] Currencies => new ICurrency[]
            {
                new CNYCurrency(), new IDRCurrency(), new IDR1000Currency(),
                new INRCurrency(), new VNDCurrency(), new VND1000Currency(),
                new THBCurrency(), new EURCurrency(), new GBPCurrency(),
                new KRWCurrency(), new JPYCurrency()
            };

            string[] GameProviders => Array.Empty<string>();

            Guid Id { get; }

            ILanguage[] Languages => new ILanguage[]
            {
                new CNLanguage(), new USLanguage(), new IDLanguage(),
                new VNLanguage(), new THLanguage()
            };

            string Name { get; }

            string OperationalEmail { get; }
        }

        internal record TgsLicensee : ILicensee
        {
            Guid ILicensee.Id => new("66C5FEF5-217B-4929-9A08-72C2479A8F4E");

            string ILicensee.Code => "TGS";

            string ILicensee.Name => "Turnkey Game System";

            string ILicensee.OperationalEmail => "tgs.services@gmail.com";

            IBrand[] ILicensee.Brands => new IBrand[]
            {
                new TgsBrand(), new GpiBrand()
            };
        }
    }
}
