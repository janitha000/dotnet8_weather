public class CityNormalizerTests
{
    private readonly CityNormalizer _sut = new(); // System Under Test

    [Theory]
    [InlineData("  Colombo  ", "colombo")]
    [InlineData("TEHRAN", "tehran")]
    public void Normalize_TrimsAndLowercases(string input, string expected)
    {
        var result = _sut.Normalize(input);
        Assert.Equal(expected, result);
    }
}