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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentity<UserModel, RoleModel>().AddEntityFrameworkStores<UserDbContext>().AddDefaultTokenProviders();

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

builder.Services.AddScoped<IBcryptCryptography, BcryptCryptography>();

builder.Services.AddSingleton(d => new SqidsEncoder<long>(new() {
    Alphabet = builder.Configuration.GetValue<string>("sqids:alphabet")!,
    MinLength = builder.Configuration.GetValue<int>("sqids:minLength"),
}));

var email = builder.Configuration.GetValue<string>("services:gmail:email");
var password = builder.Configuration.GetValue<string>("services:gmail:password");
var name = builder.Configuration.GetValue<string>("services:gmail:password");

builder.Services.AddSingleton<EmailService>(d => new EmailService(email!, password!, name!));

var signKey = builder.Configuration.GetValue<string>("jwt:signKey");

builder.Services.AddSingleton<ITokenService, TokenService>();

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
