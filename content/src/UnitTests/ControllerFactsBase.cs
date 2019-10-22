using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyVendor.MyService.Infrastructure;
using Xunit.Abstractions;

namespace MyVendor.MyService
{
    /// <summary>
    /// Sets up an in-memory version of the ASP.NET MVC stack for decoupled testing of controllers.
    /// </summary>
    public abstract class ControllerFactsBase : IDisposable
    {
        private readonly IHost _host;
        private readonly TestServer _server;

        protected ControllerFactsBase(ITestOutputHelper output)
        {
            _host = CreateHostBuilder(output).Start();
            _server = _host.GetTestServer();
            HttpHandler = _server.CreateHandler();
        }

        private IHostBuilder CreateHostBuilder(ITestOutputHelper output)
            => new HostBuilder().ConfigureWebHost(x =>
                x.UseTestServer()
                 .ConfigureLogging(builder => builder.AddXUnit(output))
                 .ConfigureServices((context, services) => services.AddTestSecurity(_claims)
                                                                   .AddRestApi())
                 .ConfigureServices(ConfigureService)
                 .Configure(builder => builder.UseAuthentication()
                                              .UseRestApi()));

        private readonly List<Claim> _claims = new List<Claim>();

        /// <summary>
        /// Treats all following requests to the test server as anonymous/unauthenticated.
        /// </summary>
        protected void AsAnonymous() => _claims.Clear();

        /// <summary>
        /// Treats all following requests to the test server as authenticated.
        /// </summary>
        protected void AsUser(string name)
        {
            _claims.Clear();
            _claims.Add(new Claim(ClaimTypes.Name, name));
        }

        /// <summary>
        /// Treats all following requests to the test server as authenticated.
        /// </summary>
        protected void AsUser(string name, params Claim[] claims)
        {
            AsUser(name);
            _claims.AddRange(claims);
        }

        /// <summary>
        /// Treats all following requests to the test server as authenticated.
        /// </summary>
        protected void AsUser(string name, params string[] scopes)
            => AsUser(name, scopes.Select(scope => new Claim(JwtClaimTypes.Scope, scope)).ToArray());

        /// <summary>
        /// Registers dependencies for controllers.
        /// </summary>
        protected abstract void ConfigureService(IServiceCollection services);

        /// <summary>
        /// A message handler configured for in-memory communication with ASP.NET MVC controllers.
        /// </summary>
        protected readonly HttpMessageHandler HttpHandler;

        public virtual void Dispose()
        {
            HttpHandler.Dispose();
            _server.Dispose();
            _host.Dispose();
        }
    }
}
