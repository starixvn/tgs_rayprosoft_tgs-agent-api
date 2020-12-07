using System;

namespace TGS.Core.DataServices.DataInitializers
{
    partial class DataInitializer
    {
        internal interface IReseller
        {
            Guid Id { get; }
            string Code { get; }
            string Name { get; }
            string OperationalEmail { get; }
            ILicensee[] Licensees { get; }           
            ICurrency[] Currencies => new ICurrency[]
            {
                new CNYCurrency(), new IDRCurrency(), new IDR1000Currency(),
                new INRCurrency(), new VNDCurrency(), new VND1000Currency(),
                new THBCurrency()
            };

            ILanguage[] Languages => new ILanguage[]
            {
                new CNLanguage(), new USLanguage(), new IDLanguage(),
                new VNLanguage(), new THLanguage()
            };

            string[] GameProviders => Array.Empty<string>();
        }

        internal record TgsReseller : IReseller
        {
            Guid IReseller.Id => new Guid("B5A2C708-AB70-4316-9F4D-5D2AD0E77460");

            string IReseller.Code => "TGS";

            string IReseller.Name => "Turnkey Game System";

            string IReseller.OperationalEmail => "tgs.services@gmail.com";

            ILicensee[] IReseller.Licensees => new ILicensee[]
            {
                new TgsLicensee()
            };
        }
    }
}