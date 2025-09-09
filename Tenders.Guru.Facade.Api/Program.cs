using System.Reflection;
using AutoMapper;
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
    .BindConfiguration(TendersApiOptions.TendersApiUrlSection);

var clientName = "TendersApiClient";
builder.Services.AddHttpClient<ITendersApiService, TendersApiService>(clientName, client =>
{
    client.BaseAddress = new Uri(builder.Configuration[TendersApiOptions.TendersApiUrlSection]!);
    client.DefaultRequestHeaders.UserAgent.ParseAdd(".NET 8");
});

// builder.Services.AddTransient<ITendersApiService, TendersApiService>();

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