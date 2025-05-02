using RuanFa.Shop.Application;
using RuanFa.Shop.Infrastructure;
using RuanFa.Shop.Web.Api;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/seeder.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services
    .AddPresentation()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UsePresentation();
Log.Information("Application Starting");

await app.RunAsync();

