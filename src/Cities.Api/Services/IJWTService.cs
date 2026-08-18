public interface IJWTService 
{
    string GenerateToken(string username, string tenantId, string role);
}