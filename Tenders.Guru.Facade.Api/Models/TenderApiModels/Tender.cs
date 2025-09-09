using System.Text.Json.Serialization;

namespace Tenders.Guru.Facade.Api.Models.TenderApiModels;

public class Tender
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("date")]
    public required string Date { get; set; }
    
    [JsonPropertyName("awarded_value_eur")]
    public required string AwardedValueEur { get; set; }
    
    [JsonPropertyName("awarded")]
    public required List<Award> Awarded { get; set; }
}