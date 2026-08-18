public class CityNormalizer : ICityNormalizer
{
    public string Normalize(string name)
    {
        return name.Trim().ToLowerInvariant();
    }
}