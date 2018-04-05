﻿using System;
using Axoom.MyService.Contacts;
using Axoom.MyService.Infrastructure;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Axoom.MyService
{
    /// <summary>
    /// Startup class used by ASP.NET Core.
    /// </summary>
    [UsedImplicitly]
    public class Startup : IStartup
    {
        public IConfigurationRoot Configuration { get; }

        /// <summary>
        /// Called by ASP.NET Core to set up an environment.
        /// </summary>
        public Startup(IHostingEnvironment env) => Configuration = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddYamlFile("appsettings.yml", optional: false, reloadOnChange: true)
            .AddYamlFile($"appsettings.{env.EnvironmentName}.yml", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        /// <summary>
        /// Called by ASP.NET Core to register services.
        /// </summary>
        public IServiceProvider ConfigureServices(IServiceCollection services) => services
            .AddInfrastructure(Configuration)
            .AddDbContext<MyServiceDbContext>(options => options.UseNpgsql(Configuration.GetSection("Database").GetValue<string>("ConnectionString")))
            .AddContacts()
            .BuildServiceProvider();

        /// <summary>
        /// Called by ASP.NET Core to configure services after they have been registered.
        /// </summary>
        public void Configure(IApplicationBuilder app)
        {
            var provider = app.UseInfrastructure();

            provider.GetRequiredService<Policies>().Startup(() =>
            {
                using (var scope = provider.CreateScope())
                    scope.ServiceProvider.GetRequiredService<MyServiceDbContext>().Database.EnsureCreated(); //.Migrate();
            });
        }
    }
}