using System.ComponentModel;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TGS.Apis.BackendOffice.Controllers.Messages;

namespace TGS.Apis.BackendOffice.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class CultureController : BackendOfficeControllerBase
    {
        private readonly IStringLocalizer _localizer;
        public CultureController(IStringLocalizer<CultureController> localizer)
        {
            _localizer = localizer;
        }

        [AllowAnonymous, IgnoreAntiforgeryToken]
        [Route("")]
        [HttpPost, Description("Change culture")]
        public MessageResponse ChangeCultureAsync(string culture)
        {
            SetCulture(culture);
            var sss = Resources.Controllers.CultureController.test;

            var s = _localizer.GetAllStrings();
            return new MessageResponse
            {
                Message = _localizer.GetString("test")
            };
        }

        private void SetCulture(string culture)
        {
            var lang = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture));
            Response.Cookies.Delete(CookieRequestCultureProvider.DefaultCookieName);
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, lang);

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
        }
    }
}
