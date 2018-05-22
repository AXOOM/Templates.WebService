using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MyVendor.MyService
{
    public class DatabaseFactsBase : IDisposable
    {
        protected readonly DbContext Context;

        protected DatabaseFactsBase()
        {
            Context = new DbContext(
                new DbContextOptionsBuilder()
                   .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use GUID so every test has its own DB
                   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                   .EnableSensitiveDataLogging()
                   .Options);
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}
