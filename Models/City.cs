using System.ComponentModel.DataAnnotations;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Latitude { get; set; } = string.Empty;
    public string Longitude { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
}

public class CreateCityDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^-?\d+(\.\d+)?$", ErrorMessage = "Latitude must be a number")]
    public string Latitude { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^-?\d+(\.\d+)?$", ErrorMessage = "Longitude must be a number")]
    public string Longitude { get; set; } = string.Empty;

    public string TimeZone { get; set; } = string.Empty;
}