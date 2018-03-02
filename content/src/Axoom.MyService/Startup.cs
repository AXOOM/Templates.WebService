﻿using System;
using Axoom.Extensions.Configuration.Yaml;
using Axoom.Extensions.Logging.Console;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            .AddLogging(builder => builder.AddConfiguration(Configuration.GetSection("Logging")))
            .AddOptions()
            .AddPolicies(Configuration.GetSection("Policies"))
            .AddMetrics()
            .AddRestApi()
            //.Configure<MyOptions>(Configuration.GetSection("MyOptions"))
            //.AddTransient<IMyService, MyService>()
            //.AddSingleton<Worker>()
            .BuildServiceProvider();

        /// <summary>
        /// Called by ASP.NET Core to configure services after they have been registered.
        /// </summary>
        public void Configure(IApplicationBuilder app)
        {
            var provider = app.ApplicationServices;

            provider.GetRequiredService<ILoggerFactory>()
                .AddAxoomConsole(Configuration.GetSection("Logging"))
                .CreateLogger<Startup>()
                .LogInformation("Starting My Service");

            //provider.GetRequiredService<IPolicies>().Startup(async () =>
            //{
            //    await provider.GetRequiredService<Worker>().StartAsync();
            //});

            provider.ExposeMetrics(port: 5000);

            app.UseRestApi();
        }
    }
}