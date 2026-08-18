using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    private readonly string? _tenantId;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenant) : base(options) 
    {
        _tenantId = tenant.TenantId;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.TenantId).HasMaxLength(64).IsRequired();
            e.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
            e.HasQueryFilter(c => _tenantId != null && c.TenantId == _tenantId);
        });
    }

    public DbSet<City> Cities  => Set<City>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
}