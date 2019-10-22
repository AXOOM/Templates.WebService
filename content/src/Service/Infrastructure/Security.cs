using System.Collections.Generic;
using System.Linq;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace MyVendor.MyService.Infrastructure
{
    public static class Security
    {
        public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            var identityOptions = configuration.Get<IdentityServerAuthenticationOptions>();
            if (string.IsNullOrEmpty(identityOptions?.Authority))
                return services;

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(configuration.Bind);

            services.AddAuthorization(IdentityServerAuthenticationDefaults.AuthenticationScheme);

//            services.ConfigureSwaggerGen(options =>
//            {
//                options.AddSecurityDefinition("oauth2-implicit", new OAuth2Scheme
//                {
//                    Type = "oauth2",
//                    Flow = "implicit",
//                    AuthorizationUrl = $"{identityOptions.Authority}/connect/authorize",
//                    Scopes = ScopeAuthorizeAttribute.GetAllScopes().ToDictionary(x => x, x => "")
//                });
//                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
//                {
//                    ["oauth2-implicit"] = new string[0]
//                });
//            });

            return services;
        }

        public static void AddAuthorization(this IServiceCollection services, string authenticationScheme)
            => services//.AddTransient<IApplicationModelProvider, AuthorizationApplicationModelProvider>()
                       .AddAuthorizationPolicyEvaluator()
                       .AddAuthorization(options =>
                        {
                            options.DefaultPolicy = new AuthorizationPolicyBuilder(authenticationScheme).RequireAuthenticatedUser().Build();
                            options.AddScopePolicies();
                        });

        /// <summary>
        /// Adds authorization policies for all scopes discovered provided by <see cref="ScopeAuthorizeAttribute.GetAllScopes"/>.
        /// </summary>
        public static void AddScopePolicies(this AuthorizationOptions options)
        {
            foreach (string scope in ScopeAuthorizeAttribute.GetAllScopes())
                options.AddPolicy(scope, ScopePolicy.Create(scope));
        }

        public static IApplicationBuilder UseSecurity(this IApplicationBuilder app)
        {
            var provider = app.ApplicationServices;

            if (provider.GetService<IAuthenticationService>() == null)
                provider.GetRequiredService<ILogger<Startup>>().LogWarning("Security is disabled.");
            else
                app.UseAuthentication();

            return app;
        }
    }
}
