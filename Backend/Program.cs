using Backend.Data;
using Backend.Filters;
using Backend.Middleware;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    options.Filters.Add<ActionLoggingFilter>());

var connStr = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connStr, o => o.UseVector()));
builder.Services.AddHttpClient<IEmbeddingService, GeminiEmbeddingService>();
builder.Services.AddHttpClient<IAiAnswerService, GeminiAiAnswerService>();
builder.Services.AddScoped<IJiebaDictService, JiebaDictService>();
builder.Services.AddScoped<ActionLoggingFilter>();
builder.Services.AddSingleton<RateLimitCounterService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
});

if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
}

app.UseWhen(
    ctx => ctx.Request.Path.StartsWithSegments("/api"),
    branch => branch.UseMiddleware<RateLimitMiddleware>());

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();
