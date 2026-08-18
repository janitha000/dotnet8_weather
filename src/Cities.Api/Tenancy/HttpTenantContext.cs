public sealed class HttpTenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _http;
    public HttpTenantContext(IHttpContextAccessor http) => _http = http;
    public string? TenantId =>
        _http.HttpContext?.User.FindFirst(TenantClaims.TenantId)?.Value;
    public bool IsResolved => !string.IsNullOrWhiteSpace(TenantId);
}