public static class CityCacheKey
{
    public static string For(string tenantId, string normalizedName) =>
        $"city:{tenantId}:{normalizedName}";
}