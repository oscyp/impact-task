using System.ComponentModel.DataAnnotations;

namespace Tenders.Guru.Facade.Api.Models;

public class PageParams
{
    [Range(0, int.MaxValue)]
    public int PageIdx { get; init; } = 0;
    [Range(1, int.MaxValue)]
    public int PageSize { get; init; } = 50;
}