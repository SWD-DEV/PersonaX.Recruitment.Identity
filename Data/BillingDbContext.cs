using Microsoft.EntityFrameworkCore;
using Persol.Marketplace.Models;

namespace Persol.Marketplace.Data;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("billings");

    }
}

