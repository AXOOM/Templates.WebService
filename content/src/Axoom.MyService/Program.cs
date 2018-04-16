﻿using Microsoft.AspNetCore.Hosting;

namespace Axoom.MyService
{
    public static class Program
    {
        public static void Main(string[] args) => BuildWebHost(args).Run();

        public static IWebHost BuildWebHost(string[] args)
            => new WebHostBuilder().UseKestrel()
                                   .UseStartup<Startup>()
                                   .Build();
    }
}
