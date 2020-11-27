using System;
using TGS.Core.Models;

namespace TGS.Core.DataServices.DataInitializers
{
    partial class DataInitializer
    {
        internal interface ILicensee
        {
            Guid Id { get; }
            string Code { get; }
            string Name { get; }
            string OperationalEmail { get; }
            ICurrency[] Currencies {  get; }
            ILanguage[] Languages {  get; }
            IBrand[] Brands { get; }
            string[] GameProviders { get; }
        }

        internal record TgsLicensee : ILicensee
        {
            Guid ILicensee.Id => new Guid("66C5FEF5-217B-4929-9A08-72C2479A8F4E");

            string ILicensee.Code => "TGS";

            string ILicensee.Name => "Turnkey Game System";

            string ILicensee.OperationalEmail => "tgs.services@gmail.com";

            ICurrency[] ILicensee.Currencies => new ICurrency[] 
            { 
                new CNYCurrency(), new IDRCurrency(), new IDR1000Currency(),
                new INRCurrency(), new VNDCurrency(), new VND1000Currency(),
                new THBCurrency()
            };

            ILanguage[] ILicensee.Languages => new ILanguage[]
            {
                new CNLanguage(), new USLanguage(), new IDLanguage(),
                new VNLanguage(), new THLanguage()
            };

            IBrand[] ILicensee.Brands => new IBrand[]
            {
                new TgsBrand()
            };

            string[] ILicensee.GameProviders => new[]
            {
                Consts.GameProvider.SBOBET
            };
        }
    }
}