// ReSharper disable InconsistentNaming

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        internal interface ICurrency : ICodeMapper
        {
        }

        internal record CNYCurrency : BaseCodeMapper, ICurrency
        {
            public CNYCurrency(string externalCode = null)
                : base("CNY", externalCode)
            {
            }
        }

        internal record EURCurrency : BaseCodeMapper, ICurrency
        {
            public EURCurrency(string externalCode = null)
                : base("EUR", externalCode)
            {
            }
        }

        internal record GBPCurrency : BaseCodeMapper, ICurrency
        {
            public GBPCurrency(string externalCode = null)
                : base("GBP", externalCode)
            {
            }
        }

        internal record IDRCurrency : BaseCodeMapper, ICurrency
        {
            public IDRCurrency(string externalCode = null)
                : base("IDR", externalCode)
            {
            }
        }

        internal record IDR1000Currency : BaseCodeMapper, ICurrency
        {
            public IDR1000Currency(string externalCode = null)
                : base("IDR_1000", externalCode)
            {
            }
        }

        internal record INRCurrency : BaseCodeMapper, ICurrency
        {
            public INRCurrency(string externalCode = null)
                : base("INR", externalCode)
            {
            }
        }

        internal record JPYCurrency : BaseCodeMapper, ICurrency
        {
            public JPYCurrency(string externalCode = null)
                : base("JPY", externalCode)
            {
            }
        }

        internal record KRWCurrency : BaseCodeMapper, ICurrency
        {
            public KRWCurrency(string externalCode = null)
                : base("KRW", externalCode)
            {
            }
        }

        internal record MYRCurrency : BaseCodeMapper, ICurrency
        {
            public MYRCurrency(string externalCode = null)
                : base("MYR", externalCode)
            {
            }
        }

        internal record SGDCurrency : BaseCodeMapper, ICurrency
        {
            public SGDCurrency(string externalCode = null)
                : base("SGD", externalCode)
            {
            }
        }

        internal record THBCurrency : BaseCodeMapper, ICurrency
        {
            public THBCurrency(string externalCode = null)
                : base("THB", externalCode)
            {
            }
        }

        internal record USDCurrency : BaseCodeMapper, ICurrency
        {
            public USDCurrency(string externalCode = null)
                : base("USD", externalCode)
            {
            }
        }

        internal record VNDCurrency : BaseCodeMapper, ICurrency
        {
            public VNDCurrency(string externalCode = null)
                : base("VND", externalCode)
            {
            }
        }

        internal record VND1000Currency : BaseCodeMapper, ICurrency
        {
            public VND1000Currency(string externalCode = null)
                : base("VND_1000", externalCode)
            {
            }
        }

        internal record HKDCurrency : BaseCodeMapper, ICurrency
        {
            public HKDCurrency(string externalCode = null)
                : base("HKD", externalCode)
            {
            }
        }
    }
}