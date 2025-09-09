using System.Text.Json.Serialization;

namespace Tenders.Guru.Facade.Api.Models.TenderApiModels;

public class SupplierInfo
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }
    
    [JsonPropertyName("slug")]
    public required object Slug { get; set; } // string or int
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}