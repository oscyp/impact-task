using System.Text.Json.Serialization;

namespace Tenders.Guru.Facade.Api.Models.TenderApiModels;

public class Award
{
    [JsonPropertyName("date")]
    public required string Date { get; set; }
    
    [JsonPropertyName("suppliers_id")]
    public required string SuppliersId { get; set; }
    
    [JsonPropertyName("suppliers")]
    public required List<SupplierInfo> Suppliers { get; set; }
}