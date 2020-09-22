namespace TGS.Core.DataServices.DataInitializers
{
    internal sealed partial class DataInitializer
    {
        internal interface ILanguage
        {
            string Code { get; }
            string ExternalCode { get; }
        }

        internal record BaseLanguage : ILanguage
        {
            private readonly string _code;
            private readonly string _externalCode;

            public BaseLanguage(string code, string externalCode)
            {
                _code = code;
                _externalCode = externalCode ?? code;
            }

            string ILanguage.Code => _code;
            string ILanguage.ExternalCode => _externalCode;
        }

        internal record USLanguage : BaseLanguage
        {
            public USLanguage(string externalCode = null) :
                base("en-US", externalCode)
            {
               
            }
        }

        internal record IDLanguage : BaseLanguage
        {
            public IDLanguage(string externalCode = null) :
                base("id-ID", externalCode)
            {
               
            }
        }

        internal record JPLanguage : BaseLanguage
        {
            public JPLanguage(string externalCode = null) :
                base("ja-JP", externalCode)
            {
               
            }
        }

        internal record THLanguage : BaseLanguage
        {
            public THLanguage(string externalCode = null) :
                base("th-TH", externalCode)
            {
               
            }
        }

        internal record KRLanguage : BaseLanguage
        {
            public KRLanguage(string externalCode = null) :
                base("ko-KR", externalCode)
            {
               
            }
        }

        internal record VNLanguage : BaseLanguage
        {
            public VNLanguage(string externalCode = null) :
                base("vi-VN", externalCode)
            {
               
            }
        }

        internal record CNLanguage : BaseLanguage
        {
            public CNLanguage(string externalCode = null) :
                base("zh-CN", externalCode)
            {
               
            }
        }
    }
}