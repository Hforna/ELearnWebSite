using User.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using User.Api.Models.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using User.Api.Services.Security.Cryptography;
using User.Api.Services.Email;
using Sqids;
using User.Api.Services.AutoMapper;
using User.Api.DbContext;
using Microsoft.Extensions.Options;
using User.Api.Services.Security.Token;
using User.Api.Filters.Token;
using Microsoft.OpenApi.Models;
using User.Api.Filters;
using User.Api.Services;
using User.Api.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<FilterBinderId>();

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
builder.Services.AddMvc(d => d.Filters.Add(typeof(FilterException)));

builder.Services.AddIdentity<UserModel, RoleModel>().AddEntityFrameworkStores<UserDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthorization(auth =>
{
    auth.AddPolicy("AdminOnly", d => d.RequireRole("admin"));
    auth.AddPolicy("CreateEmployeeAccounts", d => d.RequireRole("admin", "staff"));
});

builder.Services.AddDbContext<UserDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("sqlserver")));

builder.Services.AddRouting(d => d.LowercaseUrls = true);

builder.Services.AddScoped(d =>
{
    var sqids = d.GetRequiredService<SqidsEncoder<long>>();
    var config = new AutoMapper.MapperConfiguration(d => d.AddProfile(new MapperService(sqids)));

    return config.CreateMapper();
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IUserReadOnly, UserRepository>();
builder.Services.AddScoped<IUserWriteOnly, UserRepository>();

builder.Services.AddScoped<IProfileReadOnly, ProfileRepository>();
builder.Services.AddScoped<IProfileWriteOnly, ProfileRepository>();

builder.Services.AddScoped<IBcryptCryptography, BcryptCryptography>();

builder.Services.AddSingleton(d => new SqidsEncoder<long>(new() {
    Alphabet = builder.Configuration.GetValue<string>("sqids:alphabet")!,
    MinLength = builder.Configuration.GetValue<int>("sqids:minLength"),
}));

var email = builder.Configuration.GetValue<string>("services:gmail:email");
var password = builder.Configuration.GetValue<string>("services:gmail:password");
var name = builder.Configuration.GetValue<string>("services:gmail:password");

builder.Services.AddSingleton<EmailService>(d => new EmailService(email!, password!, name!));
builder.Services.AddSingleton<ImageService>(d => new ImageService());

var signKey = builder.Configuration.GetValue<string>("jwt:signKey");

builder.Services.AddScoped<ITokenService, TokenService>();
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

builder.Services.AddHostedService<DeleteAccountService>();

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
