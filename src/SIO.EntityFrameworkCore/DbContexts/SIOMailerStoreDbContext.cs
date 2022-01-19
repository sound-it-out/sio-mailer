using Microsoft.EntityFrameworkCore;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;

namespace SIO.EntityFrameworkCore.DbContexts
{
    public class SIOMailerStoreDbContext : SIOStoreDbContextBase<SIOMailerStoreDbContext>
    {
        public SIOMailerStoreDbContext(DbContextOptions<SIOMailerStoreDbContext> options) : base(options)
        {
        }
    }
}
