using System.Security.Claims;

namespace Umbraco13Demo.Helpers
{
    public interface IBackofficeUserAccessor
    {
        ClaimsIdentity BackofficeUser { get; }
    }
}
