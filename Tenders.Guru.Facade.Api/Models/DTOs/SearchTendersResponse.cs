namespace Tenders.Guru.Facade.Api.Models.DTOs;

public class SearchTendersResponse
{
    public required int Total { get; init; }
    public required IEnumerable<TenderDto> Tenders { get; init; }
}