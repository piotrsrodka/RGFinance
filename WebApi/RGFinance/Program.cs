using Database;
using Microsoft.EntityFrameworkCore;
using RGFinance.FlowFeature;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<RGFContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("RGF")).EnableSensitiveDataLogging());

builder.Services.AddTransient<IFlowService, FlowService>();
builder.Services.AddTransient<IForexService, ForexService>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
