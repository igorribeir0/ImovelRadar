using System.Text.Json.Serialization;
using ImovelRadar.Application.Interfaces;
using ImovelRadar.Application.Services;
using ImovelRadar.Domain.Services;
using ImovelRadar.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

// Domain Services
builder.Services.AddScoped<MarketStatisticsService>();
builder.Services.AddScoped<PriceHistoryAnalyzer>();
builder.Services.AddScoped<OpportunityScoreService>();
builder.Services.AddScoped<PropertyComparisonService>();
builder.Services.AddScoped<PropertyOpportunityAnalyzer>();
builder.Services.AddScoped<AnalysisConfidenceService>();

// Application Services
builder.Services.AddScoped<PropertyAnalysisService>();
builder.Services.AddScoped<PropertyQueryService>();
builder.Services.AddScoped<NeighborhoodStatisticsService>();
builder.Services.AddScoped<OpportunityRankingService>();

// Infrastructure
builder.Services.AddSingleton<
    IPropertyRepository,
    InMemoryPropertyRepository>();

builder.Services.AddSingleton<
    IPropertyPriceHistoryRepository,
    InMemoryPropertyPriceHistoryRepository>();

builder.Services.AddSingleton<
    ISourceRepository,
    InMemorySourceRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
