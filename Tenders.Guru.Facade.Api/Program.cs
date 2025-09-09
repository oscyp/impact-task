using Tenders.Guru.Facade.Api.Config;
using Tenders.Guru.Facade.Api.MappingProfiles;
using Tenders.Guru.Facade.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddAutoMapper(cfg => {}, typeof(TendersProfile));

builder.Services.AddOptions<TendersApiOptions>()
    .BindConfiguration(TendersApiOptions.TendersSection)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddHttpClient<ITendersService, TendersService>(TendersService.HttpClientName, client =>
{
    client.BaseAddress = new Uri(builder.Configuration[$"{TendersApiOptions.TendersSection}:{nameof(TendersApiOptions.ApiUrl)}"]!);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("tenders-service");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();