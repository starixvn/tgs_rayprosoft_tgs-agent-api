using System;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        internal interface ILicensee
        {
            Guid Id { get; }
            string Code { get; }
            string Name { get; }
            string OperationalEmail { get; }
            IBrand[] Brands { get; }

            ICurrency[] Currencies => new ICurrency[] 
            { 
                new CNYCurrency(), new IDRCurrency(), new IDR1000Currency(),
                new INRCurrency(), new VNDCurrency(), new VND1000Currency(),
                new THBCurrency(), new EURCurrency(), new GBPCurrency(),
                new KRWCurrency(), new JPYCurrency()
            };

            ILanguage[] Languages => new ILanguage[]
            {
                new CNLanguage(), new USLanguage(), new IDLanguage(),
                new VNLanguage(), new THLanguage()
            };

            string[] GameProviders => Array.Empty<string>();
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