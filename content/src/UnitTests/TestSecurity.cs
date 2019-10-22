using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyVendor.MyService.Infrastructure;

namespace MyVendor.MyService
{
    public static class TestSecurity
    {
        public static IServiceCollection AddTestSecurity(this IServiceCollection services, IEnumerable<Claim> claims)
        {
            services.AddAuthentication(TestAuthentication.Scheme)
                    .AddScheme<TestAuthenticationOptions, TestAuthentication>(
                         TestAuthentication.Scheme,
                         "Test Auth",
                         options => options.Claims = claims);

            services.AddAuthorization(TestAuthentication.Scheme);

            return services;
        }

        public class TestAuthentication : AuthenticationHandler<TestAuthenticationOptions>
        {
            public new const string Scheme = "Test";

            public TestAuthentication(IOptionsMonitor<TestAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
                : base(options, logger, encoder, clock)
            {}

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
                => Task.FromResult(Options.Claims.Any()
                    ? AuthenticateResult.Success(new AuthenticationTicket(
                        new ClaimsPrincipal(new ClaimsIdentity(Options.Claims, Scheme)),
                        new AuthenticationProperties(),
                        Scheme))
                    : AuthenticateResult.Fail("Not authenticated"));
        }

        public class TestAuthenticationOptions : AuthenticationSchemeOptions
        {
            public IEnumerable<Claim> Claims { get; set; } = new List<Claim>();
        }
    }
}
