public sealed record DemoUser(string Username, string Password, string TenantId, string Role);

public static class DemoUsers
{
    public static readonly DemoUser[] All =
    [
        new("acme", "password", "acme", "Admin"),
        new("contoso", "password", "contoso", "Admin"),
    ];

    public static DemoUser? Find(string username, string password) =>
        All.FirstOrDefault(u =>
            u.Username == username && u.Password == password);
}