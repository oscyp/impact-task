using System.ComponentModel.DataAnnotations;

namespace Tenders.Guru.Facade.Api.Config;

public class TendersApiOptions
{
    public const string TendersSection = "Tenders";
    [Required]
    public required string ApiUrl { get; set; }
    [Required]
    public required int MaxPageSize { get; set; }
}