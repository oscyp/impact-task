using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.Extensions.Options;
using Tenders.Guru.Facade.Api.Config;
using Tenders.Guru.Facade.Api.Exceptions;

namespace Tenders.Guru.Facade.Api.Services;

public interface ITendersApiService
{
    Task<TenderDto> GenTender(int tenderId, CancellationToken cancellationToken);
    Task<TendersResponse> GetTenders(SearchParams searchParams, CancellationToken cancellationToken);
    // IEnumerable<Tender> GetTendersByPrice(string currency, SearchParams searchParams);
    // IEnumerable<Tender> GetOrderedTendersByPrice(string currency, SearchParams searchParams);
    // IEnumerable<Tender> GetTendersByDate(DateTime dateTime, SearchParams searchParams);
    // IEnumerable<Tender> GetOrderedTendersByDate(DateTime dateTime, SearchParams searchParams);
    Task<IEnumerable<TenderDto>> GetWonTendersBySupplierId(int supplierId, SearchParams searchParams, CancellationToken cancellationToken);
}

public class SearchParams
{
    public PageParams PageParams { get; set; }
    public int Price { get; set; }
    public DateTime Date { get; set; }
    public string OrderBy { get; set; }
    public Order Order { get; set; } = Order.Asc;
}

public enum Order
{
    Asc,
    Desc
}

public class PageParams
{
    public int PageIdx { get; set; }
    public int PageSize { get; set; } = 20;
    public int Total { get; set; } 
}

public class Tender
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("date")]
    public string Date { get; set; }
    
    [JsonPropertyName("deadline_date")]
    public string? DeadlineDate { get; set; }
    
    [JsonPropertyName("deadline_length_days")]
    public string? DeadlineLengthDays { get; set; }
    
    [JsonPropertyName("deadline_length_hours")]
    public string? DeadlineLengthHours { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("category")]
    public string Category { get; set; }
    
    [JsonPropertyName("description")]
    public string Description { get; set; }
    
    [JsonPropertyName("sid")]
    public string Sid { get; set; }
    
    [JsonPropertyName("awarded_value")]
    public string? AwardedValue { get; set; }
    
    [JsonPropertyName("awarded_currency")]
    public string? AwardedCurrency { get; set; }
    
    [JsonPropertyName("awarded_value_eur")]
    public string? AwardedValueEur { get; set; }
    
    [JsonPropertyName("purchaser")]
    public Purchaser Purchaser { get; set; }
    
    [JsonPropertyName("type")]
    public TenderType Type { get; set; }
    
    [JsonPropertyName("indicators")]
    public List<string>? Indicators { get; set; }
    
    [JsonPropertyName("awarded")]
    public List<Award>? Awarded { get; set; }
}

public class Purchaser
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("sid")]
    public string? Sid { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class TenderType
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("slug")]
    public string Slug { get; set; }
}

public class Award
{
    [JsonPropertyName("date")]
    public string Date { get; set; }
    
    [JsonPropertyName("suppliers_id")]
    public string SuppliersId { get; set; }
    
    [JsonPropertyName("count")]
    public string Count { get; set; }
    
    [JsonPropertyName("value")]
    public string Value { get; set; }
    
    [JsonPropertyName("value_min")]
    public string ValueMin { get; set; }
    
    [JsonPropertyName("value_max")]
    public string ValueMax { get; set; }
    
    [JsonPropertyName("value_estimated")]
    public string ValueEstimated { get; set; }
    
    [JsonPropertyName("suppliers_name")]
    public string SuppliersName { get; set; }
    
    [JsonPropertyName("suppliers")]
    public List<SupplierInfo> Suppliers { get; set; }
    
    [JsonPropertyName("value_for_one")]
    public decimal ValueForOne { get; set; }
    
    [JsonPropertyName("value_for_two")]
    public decimal ValueForTwo { get; set; }
    
    [JsonPropertyName("value_for_three")]
    public decimal ValueForThree { get; set; }
    
    [JsonPropertyName("offers_count_data")]
    public Dictionary<string, OfferCountData>? OffersCountData { get; set; }
    
    [JsonPropertyName("offers_count")]
    public List<int> OffersCount { get; set; }
    
    [JsonPropertyName("offers_count_status")]
    public string? OffersCountStatus { get; set; }
    
    [JsonPropertyName("value_eur")]
    public decimal ValueEur { get; set; }
    
    [JsonPropertyName("value_for_one_eur")]
    public decimal ValueForOneEur { get; set; }
    
    [JsonPropertyName("value_for_two_eur")]
    public decimal ValueForTwoEur { get; set; }
    
    [JsonPropertyName("value_for_three_eur")]
    public decimal ValueForThreeEur { get; set; }
}

public class SupplierInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("slug")]
    public string Slug { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class OfferCountData
{
    [JsonPropertyName("count")]
    public string Count { get; set; }
    
    [JsonPropertyName("value")]
    public string Value { get; set; }
    
    [JsonPropertyName("value_eur")]
    public decimal ValueEur { get; set; }
}

public class TendersResponse
{
    [JsonPropertyName("page_count")]
    public int PageCount { get; set; }
    
    [JsonPropertyName("page_number")]
    public int PageNumber { get; set; }
    
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }
    
    [JsonPropertyName("total")]
    public int Total { get; set; }
    
    [JsonPropertyName("data")]
    public List<Tender> Data { get; set; }
}

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class TenderDto
{
    public int Id { get; set; }
    public string Date { get; set; }
    public string? DeadlineDate { get; set; }
    public string Title { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public string Sid { get; set; }
    public decimal? AwardedValueEur { get; set; }
    public string? AwardedCurrency { get; set; }
    public PurchaserDto Purchaser { get; set; }
    public TenderTypeDto Type { get; set; }
    public List<string>? Indicators { get; set; }
    public List<AwardDto>? Awarded { get; set; }
}

public class PurchaserDto
{
    public int Id { get; set; }
    public string? Sid { get; set; }
    public string? Name { get; set; }
}

public class TenderTypeDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
}

public class AwardDto
{
    public string Date { get; set; }
    public string SuppliersId { get; set; }
    public int Count { get; set; }
    public decimal Value { get; set; }
    public decimal ValueEur { get; set; }
    public string SuppliersName { get; set; }
    public List<SupplierDto> Suppliers { get; set; }
}

public class SupplierDto
{
    public int Id { get; set; }
    public string Slug { get; set; }
    public string Name { get; set; }
}

public class TendersApiService(HttpClient httpClient, IMapper mapper) : ITendersApiService
{
    private const string DefaultCurrency = "EUR";
    
    public async Task<TenderDto> GenTender(int tenderId, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetFromJsonAsync<Tender>(tenderId.ToString(), cancellationToken);

        if (response is null)
        {
            throw new TendersApiException($"Tender {tenderId} not found");
        }

        return mapper.Map<TenderDto>(response);
    }

    public async Task<TendersResponse> GetTenders(SearchParams searchParams, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TenderDto>> GetWonTendersBySupplierId(int supplierId, SearchParams searchParams, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}