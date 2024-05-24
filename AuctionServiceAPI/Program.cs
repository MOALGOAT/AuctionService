using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AuctionServiceAPI.Controllers;
using AuctionServiceAPI.Models;
using AuctionServiceAPI.Service;
using NLog;
using NLog.Web;
using AuctionServiceAPI;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings()
    .GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    var configuration=builder.Configuration;
    var vaultService=new VaultService(configuration);
    string connectionString=await vaultService.GetConnectionStringAsync("secrets", "MongoConnectionString");
    configuration["MongoConnectionString"]=connectionString;
    Console.WriteLine("Jeg hedder Furkan 123"+connectionString);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers();
    builder.Services.AddTransient<VaultService>();
    builder.Services.AddSingleton<IAuctionService, AuctionMongoDBService>();
    builder.Services.AddSingleton<MongoDBContext>();
    builder.Services.AddHostedService<BidReceiver>();

   




    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();
    app.UseHttpsRedirection();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}
