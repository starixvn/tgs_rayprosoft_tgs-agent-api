using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        internal interface IReseller
        {
            Guid Id { get; }
            string Code { get; }
            string Name { get; }
            string OperationalEmail { get; }
            BusinessType BusinessType { get; }
            ILicensee[] Licensees { get; }           
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

        internal record TgsReseller : IReseller
        {
            Guid IReseller.Id => new Guid("B5A2C708-AB70-4316-9F4D-5D2AD0E77460");

            string IReseller.Code => "TGS";

            string IReseller.Name => "Turnkey Game System";

            string IReseller.OperationalEmail => "tgs.services@gmail.com";

            BusinessType IReseller.BusinessType => BusinessType.B2C;

            ILicensee[] IReseller.Licensees => new ILicensee[]
            {
                new TgsLicensee()
            };
        }
    }
}