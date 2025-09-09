using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Tenders.Guru.Facade.Api.Config;
using Tenders.Guru.Facade.Api.Exceptions;
using Tenders.Guru.Facade.Api.Models;
using Tenders.Guru.Facade.Api.Models.DTOs;
using Tenders.Guru.Facade.Api.Models.TenderApiModels;

namespace Tenders.Guru.Facade.Api.Services;

public interface ITendersService
{
    Task<TenderDto> GenTender(int tenderId, CancellationToken cancellationToken);
    Task<SearchTendersResponse> SearchTenders(SearchParams searchParams, CancellationToken cancellationToken);
}

public class TendersService(HttpClient httpClient, IMapper mapper, IMemoryCache memoryCache, IOptions<TendersApiOptions> options) : ITendersService
{
    public const string HttpClientName = "TendersApiClient";
    private const string TendersEndpoint = "tenders";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);
    private int PageLimitation => options.Value.MaxPageSize;
    
    public async Task<TenderDto> GenTender(int tenderId, CancellationToken cancellationToken)
    {
        var cacheKey = $"tender_{tenderId}";
        
        if (!memoryCache.TryGetValue(cacheKey, out Tender? response))
        {
            response = await httpClient.GetFromJsonAsync<Tender>($"{TendersEndpoint}/{tenderId.ToString()}", cancellationToken);

            if (response is null)
            {
                throw new TendersApiException($"Tender {tenderId} not found");
            }

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpiration,
                Priority = CacheItemPriority.Normal
            };
            
            memoryCache.Set(cacheKey, response, cacheOptions);
        }

        return mapper.Map<TenderDto>(response!);
    }
    
    public async Task<SearchTendersResponse> SearchTenders(SearchParams searchParams, CancellationToken cancellationToken)
    {
        const string cacheKey = "tenders_response";
        
        if (!memoryCache.TryGetValue(cacheKey, out List<Tender>? allTenders))
        {
            allTenders = new List<Tender>();
            
            var tasks = new List<Task<TendersResponse?>>();
            
            for (int i = 1; i <= PageLimitation; i++) // api starts page index from 1
            {
                tasks.Add(httpClient.GetFromJsonAsync<TendersResponse>($"{TendersEndpoint}?page={i}", cancellationToken));
            }
            
            var responses = await Task.WhenAll(tasks);
            
            foreach (var response in responses)
            {
                if (response is null)
                {
                    throw new TendersApiException("No tenders found");
                }
                
                allTenders.AddRange(response.Data);
            }

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpiration,
                Priority = CacheItemPriority.High
            };
            
            memoryCache.Set(cacheKey, allTenders, cacheOptions);
        }

        IEnumerable<Tender> tenders = allTenders!;

        tenders = ApplySearch(tenders, searchParams);

        if (!string.IsNullOrEmpty(searchParams.OrderBy))
        {
            tenders = ApplyOrdering(tenders, searchParams.OrderBy, searchParams.Order);
        }

        var filteredTenders = tenders.ToList();

        var pagedTenders = filteredTenders
            .Skip(searchParams.PageParams.PageSize * searchParams.PageParams.PageIdx)
            .Take(searchParams.PageParams.PageSize * searchParams.PageParams.PageIdx + searchParams.PageParams.PageSize)
            .ToList();

        var mappedTenders = mapper.Map<IEnumerable<TenderDto>>(pagedTenders);

        return new SearchTendersResponse() { Tenders = mappedTenders, Total = filteredTenders.Count };
    }


    private static IEnumerable<Tender> ApplySearch(IEnumerable<Tender> tenders, SearchParams searchParams)
    {
        if (searchParams.PriceInEur is not null)
        {
            tenders = tenders.Where(x => x.AwardedValueEur == searchParams.PriceInEur.ToString());
        }

        if (searchParams.Date.HasValue)
        {
            tenders = tenders.Where(x => x.Date == searchParams.Date.Value.ToString("yyyy-MM-dd"));
        }

        if (searchParams.SupplierId.HasValue)
        {
            tenders = tenders.Where(tender => tender.Awarded.Any(award => award.Suppliers.Any(supplier => supplier.Id == searchParams.SupplierId.Value)));
        }

        return tenders;
    }

    private static IEnumerable<Tender> ApplyOrdering(IEnumerable<Tender> tenders, string orderBy, Order? order)
    {
        var isDescending = order == Order.Desc;
        
        return orderBy.ToLower() switch
        {
            "date" => isDescending ? tenders.OrderByDescending(x => x.Date) : tenders.OrderBy(x => x.Date),
            "price_eur" => isDescending ? tenders.OrderByDescending(x => x.AwardedValueEur) : tenders.OrderBy(x => x.AwardedValueEur),
            _ => tenders
        };
    }
}