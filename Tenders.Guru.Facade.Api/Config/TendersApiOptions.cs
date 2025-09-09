namespace Tenders.Guru.Facade.Api.Config;

public class TendersApiOptions
{
    public const string TendersApiUrlSection = "TendersApiUrl";
    public required string TendersApiUrl { get; set; }
}