using Database;
using Microsoft.EntityFrameworkCore;
using RGFinance.FlowFeature;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<RGFContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("RGF")).EnableSensitiveDataLogging());

builder.Services.AddTransient<IFlowService, FlowService>();
builder.Services.AddTransient<IForexService, ForexService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

var app = builder.Build();

// Auto-migrate database on startup (for Docker)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RGFContext>();
    try
    {
        context.Database.EnsureCreated(); // Creates database if not exists
        // Alternatively use: context.Database.Migrate(); if you have migrations
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the database.");
    }
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
