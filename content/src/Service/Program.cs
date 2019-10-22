using Axoom.Extensions.Logging.Console;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MyVendor.MyService
{
    // Manage process lifecycle, configuration and logging
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        [PublicAPI]
        public static IHostBuilder CreateHostBuilder(string[] args)
            => new HostBuilder().ConfigureWebHost(x =>
                x.UseKestrel()
                 .ConfigureAppConfiguration((context, builder) =>
                  {
                      var env = context.HostingEnvironment;
                      builder.SetBasePath(env.ContentRootPath)
                             .AddYamlFile("appsettings.yml", optional: false, reloadOnChange: true)
                             .AddYamlFile($"appsettings.{env.EnvironmentName}.yml", optional: true, reloadOnChange: true)
                             .AddEnvironmentVariables()
                             .AddUserSecrets<Startup>()
                             .AddCommandLine(args);
                  })
                 .ConfigureLogging((context, builder) =>
                  {
                      var config = context.Configuration.GetSection("Logging");
                      builder.AddConfiguration(config)
                             .AddAxoomConsole(config)
                             .AddExceptionDemystifyer();
                  })
                 .UseStartup<Startup>());
    }
}
