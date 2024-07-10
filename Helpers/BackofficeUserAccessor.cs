using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Umbraco13Demo.Helpers
{
    /// <summary>
    /// https://our.umbraco.com/forum/umbraco-9/106857-how-do-i-determine-if-a-backoffice-user-is-logged-in-from-a-razor-view
    /// </summary>
    public class BackofficeUserAccessor : IBackofficeUserAccessor
    {
        private readonly IOptionsSnapshot<CookieAuthenticationOptions> _cookieOptionsSnapshot;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BackofficeUserAccessor(
            IOptionsSnapshot<CookieAuthenticationOptions> cookieOptionsSnapshot,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _cookieOptionsSnapshot = cookieOptionsSnapshot;
            _httpContextAccessor = httpContextAccessor;
        }


        public ClaimsIdentity BackofficeUser
        {
            get
            {

                var httpContext = _httpContextAccessor.HttpContext;

                if (httpContext == null)
                    return new ClaimsIdentity();


                CookieAuthenticationOptions cookieOptions = _cookieOptionsSnapshot.Get(Umbraco.Cms.Core.Constants.Security.BackOfficeAuthenticationType);
                string backOfficeCookie = httpContext.Request.Cookies[cookieOptions.Cookie.Name!];

                if (string.IsNullOrEmpty(backOfficeCookie))
                    return new ClaimsIdentity();

                AuthenticationTicket unprotected = cookieOptions.TicketDataFormat.Unprotect(backOfficeCookie!);
                ClaimsIdentity backOfficeIdentity = unprotected!.Principal.GetUmbracoIdentity();

                return backOfficeIdentity;
            }

        }
    }
}
