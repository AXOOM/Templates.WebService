using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MyVendor.MyService.Infrastructure
{
    public static class Database
    {
        public static void AddDatabase(this IServiceCollection services, string connectionString)
            => services.AddDbContext<DbContext>(options =>
                        {
                            if (connectionString.Contains("Host=")) options.UseNpgsql(connectionString);
                            else options.UseSqlite(connectionString);
                        })
                       .AddHostedService<DatabaseInit>();
    }

    public class DatabaseInit : IHostedService
    {
        private readonly IServiceProvider _services;

        public DatabaseInit(IServiceProvider services)
        {
            _services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            await scope.ServiceProvider.GetRequiredService<DbContext>().Database.EnsureCreatedAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
