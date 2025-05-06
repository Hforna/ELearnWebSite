using Progress.Api.Middlewares;
using Progress.Infrastructure;
using Progress.Application;
using Course.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

builder.Services.AddTransient<IdValidtorMiddleware>();

app.UseHttpsRedirection();

app.UseMiddleware<CultureInfoMiddleware>();

app.UseMiddleware<IdValidtorMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
