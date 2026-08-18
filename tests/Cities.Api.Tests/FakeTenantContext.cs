public sealed class FakeTenantContext : ITenantContext
{
    public FakeTenantContext(string tenantId) => TenantId = tenantId;

    public string? TenantId { get; }
    public bool IsResolved => !string.IsNullOrWhiteSpace(TenantId);
}
