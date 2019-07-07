using Application.Identity.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Identity.Api.Data
{
    public class IdentityContext : DbContext
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
