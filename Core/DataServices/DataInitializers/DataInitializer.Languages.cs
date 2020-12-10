namespace TGS.Core.DataServices.DataInitializers
{
    partial class DataInitializer
    {
        internal interface ILanguage : ICodeMapper
        {
        }

        internal record USLanguage : BaseCodeMapper, ILanguage
        {
            public USLanguage(string externalCode = null) :
                base("en-US", externalCode)
            {
               
            }
        }

        internal record IDLanguage : BaseCodeMapper, ILanguage
        {
            public IDLanguage(string externalCode = null) :
                base("id-ID", externalCode)
            {
               
            }
        }

        internal record JPLanguage : BaseCodeMapper, ILanguage
        {
            public JPLanguage(string externalCode = null) :
                base("ja-JP", externalCode)
            {
               
            }
        }

        internal record THLanguage : BaseCodeMapper, ILanguage
        {
            public THLanguage(string externalCode = null) :
                base("th-TH", externalCode)
            {
               
            }
        }

        internal record KRLanguage : BaseCodeMapper, ILanguage
        {
            public KRLanguage(string externalCode = null) :
                base("ko-KR", externalCode)
            {
               
            }
        }

        internal record VNLanguage : BaseCodeMapper, ILanguage
        {
            public VNLanguage(string externalCode = null) :
                base("vi-VN", externalCode)
            {
               
            }
        }

        internal record CNLanguage : BaseCodeMapper, ILanguage
        {
            public CNLanguage(string externalCode = null) :
                base("zh-CN", externalCode)
            {
               
            }
        }

        internal record TWLanguage : BaseCodeMapper, ILanguage
        {
            public TWLanguage(string externalCode = null) :
                base("zh-TW", externalCode)
            {
                
            }
        }
    }
}