using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using payment_api.Connection;
using payment_api.Controller;

namespace payment_api.test.Helpers
{
    public class MoqDb : IDbContextFactory<VendasContext>
    {
        public VendasContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<VendasContext>()
                .UseInMemoryDatabase($"InMemoryTestDb-{DateTime.Now.ToFileTimeUtc}")
                .Options;

            return new VendasContext( options );
        }
    }
}