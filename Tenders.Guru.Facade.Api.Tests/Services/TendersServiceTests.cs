using System.Net;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Tenders.Guru.Facade.Api.Config;
using Tenders.Guru.Facade.Api.Exceptions;
using Tenders.Guru.Facade.Api.MappingProfiles;
using Tenders.Guru.Facade.Api.Models;
using Tenders.Guru.Facade.Api.Models.TenderApiModels;
using Tenders.Guru.Facade.Api.Services;
using Tenders.Guru.Facade.Api.Tests.Testing;

namespace Tenders.Guru.Facade.Api.Tests.Services;

[TestFixture]
public class TendersServiceTests
{
    private IMapper _mapper = null!;
    private IMemoryCache _memoryCache = null!;
    private IOptions<TendersApiOptions> _options = null!;
    private Mock<ILoggerFactory> _loggerFactory = null!;
    [SetUp]
    public void SetUp()
    {
        var mockLogger = new Mock<ILogger>();
        _loggerFactory = new Mock<ILoggerFactory>();

        _loggerFactory
            .Setup(f => f.CreateLogger(It.IsAny<string>()))
            .Returns(mockLogger.Object);
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TendersProfile>();
        }, _loggerFactory.Object);
        
        _mapper = mapperConfig.CreateMapper();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _options = Options.Create(new TendersApiOptions { ApiUrl = "https://tenders.guru/api/pl/", MaxPageSize = 100 });
    }

    [Test]
    public async Task Given_CachedTenders_When_FilterByDate_Then_ReturnsOnlyMatchingDate()
    {
        // Arrange
        var tenders = new List<Tender>
        {
            new Tender { Id = 1, Date = "2024-01-01", AwardedValueEur = "10", Title = "title1", Awarded = new List<Award> { new Award { Date = "2024-01-01", SuppliersId = "1", Suppliers = new List<SupplierInfo>() } } },
            new Tender { Id = 2, Date = "2024-02-01", AwardedValueEur = "20", Title = "title2", Awarded = new List<Award> { new Award { Date = "2024-02-01", SuppliersId = "2", Suppliers = new List<SupplierInfo>() } } },
        };
        _memoryCache.Set("tenders_response", tenders);
        var service = CreateService();
        var @params = new SearchTendersParams { Date = new DateOnly(2024, 1, 1), PageParams = new PageParams { PageIdx = 0, PageSize = 50 } };

        // Act
        var result = await service.SearchTenders(@params, CancellationToken.None);

        // Assert
        Assert.That(result.Tenders.Count(), Is.EqualTo(1));
        Assert.That(result.Tenders.First().Id, Is.EqualTo(1));
    }

    [Test]
    public async Task Given_CachedTenders_When_FilterBySupplierId_Then_ReturnsOnlyMatching()
    {
        // Arrange
        var tenders = new List<Tender>
        {
            new Tender { Id = 1, Date = "2024-01-01", AwardedValueEur = "10", Title = "title",  Awarded = new List<Award> { new Award { Date = "2024-01-01", SuppliersId = "1", Suppliers = new List<SupplierInfo>{ new SupplierInfo { Id = 11, Slug = "a", Name = "A" } } } } },
            new Tender { Id = 2, Date = "2024-01-01", AwardedValueEur = "20", Title = "title",  Awarded = new List<Award> { new Award { Date = "2024-01-01", SuppliersId = "2", Suppliers = new List<SupplierInfo>{ new SupplierInfo { Id = 22, Slug = "b", Name = "B" } } } } },
        };
        _memoryCache.Set("tenders_response", tenders);
        var service = CreateService();
        var @params = new SearchTendersParams { SupplierId = 22, PageParams = new PageParams { PageIdx = 0, PageSize = 50 } };

        // Act
        var result = await service.SearchTenders(@params, CancellationToken.None);

        // Assert
        Assert.That(result.Tenders.Count(), Is.EqualTo(1));
        Assert.That(result.Tenders.First().Id, Is.EqualTo(2));
    }

    [Test]
    public async Task Given_CachedTenders_When_Paginating_Then_ReturnsCorrectPage()
    {
        // Arrange
        var tenders = Enumerable.Range(1, 5).Select(i =>
            new Tender
            {
                Id = i,
                Date = $"2024-01-0{i}",
                AwardedValueEur = i.ToString(),
                Title = "title",
                Awarded = new List<Award> { new Award { Date = $"2024-01-0{i}", SuppliersId = i.ToString(), Suppliers = new List<SupplierInfo>() } }
            }).ToList();
        _memoryCache.Set("tenders_response", tenders);
        var service = CreateService();
        var @params = new SearchTendersParams { PageParams = new PageParams { PageIdx = 1, PageSize = 2 } };

        // Act
        var result = await service.SearchTenders(@params, CancellationToken.None);

        // Assert
        var ids = result.Tenders.Select(t => t.Id).ToArray();
        Assert.That(ids, Is.EqualTo(new[] { 3, 4 }));
        Assert.That(result.Total, Is.EqualTo(5));
    }

    [Test]
    public async Task Given_CachedTenders_When_OrderByDateDesc_Then_ReturnsSorted()
    {
        // Arrange
        var tenders = new List<Tender>
        {
            new Tender { Id = 1, Date = "2024-01-01", AwardedValueEur = "10", Title = "title",  Awarded = new List<Award> { new Award { Date = "2024-01-01", SuppliersId = "1", Suppliers = new List<SupplierInfo>() } } },
            new Tender { Id = 2, Date = "2024-03-01", AwardedValueEur = "20", Title = "title", Awarded = new List<Award> { new Award { Date = "2024-03-01", SuppliersId = "2", Suppliers = new List<SupplierInfo>() } } },
            new Tender { Id = 3, Date = "2024-02-01", AwardedValueEur = "30", Title = "title", Awarded = new List<Award> { new Award { Date = "2024-02-01", SuppliersId = "3", Suppliers = new List<SupplierInfo>() } } },
        };
        _memoryCache.Set("tenders_response", tenders);
        var service = CreateService();
        var @params = new SearchTendersParams { OrderBy = "date", Order = Order.Desc, PageParams = new PageParams { PageIdx = 0, PageSize = 10 } };

        // Act
        var result = await service.SearchTenders(@params, CancellationToken.None);

        // Assert
        var ids = result.Tenders.Select(t => t.Id).ToArray();
        Assert.That(ids, Is.EqualTo(new[] { 2, 3, 1 }));
    }

    [Test]
    public async Task Given_CachedTender_When_GetById_Then_ReturnsFromCache()
    {
        // Arrange
        var tender = new Tender { Id = 123, Date = "2024-01-10", AwardedValueEur = "99", Title = "title",  Awarded = new List<Award> { new Award { Date = "2024-01-10", SuppliersId = "1", Suppliers = new List<SupplierInfo>() } } };
        _memoryCache.Set("tender_123", tender);
        var service = CreateService();

        // Act
        var dto = await service.GetTender(123, CancellationToken.None);

        // Assert
        Assert.That(dto.Id, Is.EqualTo(123));
        Assert.That(dto.Date, Is.EqualTo("2024-01-10"));
    }

    [Test]
    public async Task Given_NoCache_When_GetTenderById_Then_CallsHttpAndReturnsDto()
    {
        // Arrange
        var tender = new Tender 
        { 
            Id = 456, 
            Date = "2024-01-15", 
            Title = "Mock Tender", 
            Description = "Mock Description",
            AwardedValueEur = "150.50", 
            Awarded = new List<Award> 
            { 
                new Award 
                { 
                    Date = "2024-01-15", 
                    SuppliersId = "1", 
                    Suppliers = new List<SupplierInfo> 
                    { 
                        new SupplierInfo { Id = 100, Slug = "test", Name = "Test Supplier" } 
                    } 
                } 
            } 
        };
        var httpClient = MockHttpMessageHandler.CreateWithResponse(tender);
        var service = new TendersService(httpClient, _mapper, _memoryCache, _options);

        // Act
        var result = await service.GetTender(456, CancellationToken.None);

        // Assert
        Assert.That(result.Id, Is.EqualTo(456));
        Assert.That(result.Title, Is.EqualTo("Mock Tender"));
        Assert.That(result.Description, Is.EqualTo("Mock Description"));
        Assert.That(result.AmountInEur, Is.EqualTo(150.50m));
        Assert.That(result.Suppliers.Count, Is.EqualTo(1));
        Assert.That(result.Suppliers.First().Name, Is.EqualTo("Test Supplier"));
    }

    [Test]
    public Task Given_NoCache_When_GetTenderByIdNotFound_Then_ThrowsException()
    {
        // Arrange
        var httpClient = MockHttpMessageHandler.CreateWithNotFound();
        var service = new TendersService(httpClient, _mapper, _memoryCache, _options);

        // Act & Assert
        var ex = Assert.ThrowsAsync<TendersApiException>(() => 
            service.GetTender(999, CancellationToken.None));
        Assert.That(ex?.Message, Does.Contain("Error fetching data from tenders/999"));
        Assert.That(ex?.InnerException, Is.TypeOf<HttpRequestException>());
        return Task.CompletedTask;
    }

    private TendersService CreateService()
    {
        var httpClient = MockHttpMessageHandler.CreateWithResponse(new object());
        return new TendersService(httpClient, _mapper, _memoryCache, _options);
    }
}


