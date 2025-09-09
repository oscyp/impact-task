namespace Tenders.Guru.Facade.Api.Models.DTOs;

public class TenderDto
{
    public int Id { get; init; }
    public required string Date { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required decimal AwardedValueEur { get; init; }
    public required List<SupplierDto> Suppliers { get; init; }
}