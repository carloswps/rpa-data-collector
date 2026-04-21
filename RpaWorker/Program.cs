using Microsoft.EntityFrameworkCore;
using Polly;
using RpaWorker;
using RpaWorker.Application;
using RpaWorker.Domain.Interfaces;
using RpaWorker.Infrastructure.Parsers;
using RpaWorker.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<ScrapingOptions>(builder.Configuration.GetSection("Scraping"));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
builder.Services.AddScoped<ICollectRepository, CollectRepository>();
builder.Services.AddSingleton<IDataParser, PriceParser>();
builder.Services.AddHttpClient<IScrapingService, ScrapingService>(client =>
    {
        client.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
        );
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddTransientHttpErrorPolicy(policy =>
        policy.WaitAndRetryAsync(
            3,
            attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))
        )
    )
    .AddTransientHttpErrorPolicy(policy =>
        policy.CircuitBreakerAsync(
            5,
            TimeSpan.FromSeconds(30)
        )
    );

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

host.Run();