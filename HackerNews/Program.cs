using HackerNews.Infrastructure;
using HackerNews.Services;
using HackerNews.Services.Cache;
using HackerNews.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;
builder.Services.Configure<HackerNewsApiOptions>(configuration.GetSection("HackerNewsApi"));

// Add caching
builder.Services.AddMemoryCache();

//Register services
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(sp.GetRequiredService<IOptions<HackerNewsApiOptions>>().Value.RedisHost));

//Register different cache engine dependent on settings
builder.Services.AddTransient<ICacheEngine>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<HackerNewsApiOptions>>().Value;

    return settings.UseRedis switch
    {
        true => new RedisCacheEngine(sp.GetRequiredService<IConnectionMultiplexer>()),
        false => new InMemoryCacheEngine(sp.GetRequiredService<IMemoryCache>()),
    };
});
builder.Services.AddTransient<ICacheEngine, InMemoryCacheEngine>();
builder.Services.AddTransient<IBestStoriesService, BestStoriesService>();
builder.Services.AddTransient<IHackerNewsClient, HackerNewsClient>();
builder.Services.AddHttpClient<IRestApiClient, RestApiClient>();

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
