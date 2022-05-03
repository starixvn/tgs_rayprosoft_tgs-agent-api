using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.DataInitializers;

public partial class DataInitializer
{
    internal interface IReseller
    {
        BusinessType BusinessType { get; }

        string Code { get; }

        ICurrency[] Currencies => new ICurrency[]
        {
            new CNYCurrency(),
            new IDRCurrency(),
            new IDR1000Currency(),
            new INRCurrency(),
            new VNDCurrency(),
            new VND1000Currency(),
            new THBCurrency(),
            new EURCurrency(),
            new GBPCurrency(),
            new KRWCurrency(),
            new JPYCurrency(),
            new HKDCurrency()
        };

        string[] GameProviders => Array.Empty<string>();

        Guid Id { get; }

        ILanguage[] Languages => new ILanguage[]
        {
            new CNLanguage(),
            new USLanguage(),
            new IDLanguage(),
            new VNLanguage(),
            new THLanguage()
        };

        ILicensee[] Licensees { get; }

        string Name { get; }

        string OperationalEmail { get; }
    }

    internal record TgsReseller : IReseller
    {
        Guid IReseller.Id => new("B5A2C708-AB70-4316-9F4D-5D2AD0E77460");

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
