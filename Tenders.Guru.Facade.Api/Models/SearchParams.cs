namespace Tenders.Guru.Facade.Api.Models;

public class SearchParams
{
    public PageParams PageParams { get; set; } = new() { PageIdx = 0, PageSize = 50 };
    
    public double? PriceInEur { get; set; }
    public DateOnly? Date { get; set; }
    public string? OrderBy { get; set; }
    public Order? Order { get; set; }
    public int? SupplierId { get; set; }
}