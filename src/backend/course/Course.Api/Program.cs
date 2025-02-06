using Course.Infrastructure;
using Course.Application;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Course.Domain.Services.Token;
using Course.Api.Filters;
using Microsoft.OpenApi.Models;
using Course.Domain.Repositories;
using Course.Api.Controllers;
using Course.Api.BackgroundServices;
using Microsoft.AspNetCore.RateLimiting;
using Course.Domain.Sessions;
using Course.Api.Sessions;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<FilterBindId>();

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

builder.Services.AddAuthorization(auth =>
{
    auth.AddPolicy("AdminOnly", d => d.RequireRole("admin"));
    auth.AddPolicy("CreateEmployeeAccounts", d => d.RequireRole("admin", "staff"));
    auth.AddPolicy("TeacherOnly", d => d.RequireRole("teacher"));
});

var signKey = builder.Configuration.GetValue<string>("jwt:signKey");

builder.Services.AddScoped<ICoursesSession, CoursesSession>();

builder.Services.AddHostedService<DeleteModuleService>();
builder.Services.AddHostedService<DeleteCourseService>();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddRateLimiter(d =>
{
    d.AddFixedWindowLimiter(policyName: "createCourseLimiter", d =>
    {
        d.PermitLimit = 3;
        d.Window = TimeSpan.FromMinutes(2);
        d.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        d.QueueLimit = 4;
    });
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ITokenReceptor, TokenReceptor>();

var tokenValidationParams = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signKey!)),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    RequireExpirationTime = false,
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt => {
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParams;
});

builder.Services.AddHttpClient("user.api", client =>
{
    client.BaseAddress = new Uri("https://user.api:8081");
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
});


builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true;
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/HealthCheck");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
