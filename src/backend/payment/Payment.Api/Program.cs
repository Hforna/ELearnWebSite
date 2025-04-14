using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Payment.Api.BackgroundServices;
using Payment.Api.Filters;
using Payment.Api.Middlewares;
using Payment.Api.Sessions;
using Payment.Application;
using Payment.Domain.Services.Rest;
using Payment.Domain.Services.Session;
using Payment.Domain.Token;
using Payment.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                        Enter 'Bearer' [space] and then your token in the text input below.
                        Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddRouting(d => d.LowercaseUrls = true);

builder.Services.AddMvc(d => d.Filters.Add(typeof(FilterException)));

builder.Services.AddHostedService<AllowBalanceService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

builder.Services.AddRateLimiter(cfg =>
{
    cfg.AddFixedWindowLimiter("PixPayment", cfg =>
    {
        cfg.PermitLimit = 3;
        cfg.Window = TimeSpan.FromMinutes(2);
        cfg.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        cfg.QueueLimit = 3;
    });
});

builder.Services.AddRateLimiter(cfg =>
{
    cfg.AddFixedWindowLimiter("CardPayment", cfg =>
    {
        cfg.Window = TimeSpan.FromMinutes(2);
        cfg.PermitLimit = 4;
        cfg.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        cfg.QueueLimit = 4;
    });
});

builder.Services.AddAuthorization(d =>
{
    d.AddPolicy("TeacherOnly", f => f.RequireRole("teacher"));
});

builder.Services.AddCors(cfg =>
{
    cfg.AddPolicy("StripeWebhook", policy => policy.WithOrigins("3.18.12.0/23", "3.130.192.0/22").WithMethods("POST"));
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(d =>
{
    d.IdleTimeout = TimeSpan.FromDays(7);
    d.Cookie.HttpOnly = true;
    d.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IOrderSessionService, OrderSessionService>();

builder.Services.AddScoped<ITokenReceptor, TokenReceptor>();

builder.Services.AddHttpClient("user.api", client =>
{
    client.BaseAddress = new Uri("https://user.api:8081");
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
});

builder.Services.AddHttpClient("course.api", client =>
{
    client.BaseAddress = new Uri("https://course.api:8083");
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CultureInfoMiddleware>();

//app.MapGet("/get-current-location", async () =>
//{
//    var locationService = builder.Services.BuildServiceProvider().GetRequiredService<ILocationRestService>();
//
//    return await locationService.GetCurrencyByUserLocation();
//});

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.UseSession();

app.MapControllers();

app.Run();

public partial class Program
{

}
