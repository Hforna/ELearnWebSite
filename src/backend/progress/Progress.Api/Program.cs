using Progress.Api.Middlewares;
using Progress.Infrastructure;
using Progress.Application;
using Course.Api.Middlewares;
using Progress.Domain.Token;
using Progress.Api.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Progress.Infrastructure.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITokenReceptor, TokenReceptor>();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddHostedService<UserDeletedSubscriber>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(d =>
{
    d.IdleTimeout = TimeSpan.FromDays(1);
});

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

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

builder.Services.AddTransient<IdValidtorMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<CultureInfoMiddleware>();

app.UseMiddleware<IdValidtorMiddleware>();

app.UseSession();

app.UseAuthorization();

app.MapControllers();

app.Run();
