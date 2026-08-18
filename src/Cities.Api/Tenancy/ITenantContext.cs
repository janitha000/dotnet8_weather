public interface ITenantContext
{
    string? TenantId { get; }
    bool IsResolved { get; }
}