using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Encodings.Web;

namespace MarthasLibrary.Tests.Common.Helpers;

public class AutoFail : AuthenticationHandler<AutoFailOptions>
{
    public const string SchemeName = "AutoFail";

    public AutoFail(IOptionsMonitor<AutoFailOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        return Task.FromResult(AuthenticateResult.Fail("AutoFail"));
    }
}

public class AutoFailOptions : AuthenticationSchemeOptions
{ }