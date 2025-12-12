using Microsoft.EntityFrameworkCore;
using SkillSwap.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// DB
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? "Server=127.0.0.1;Port=3306;Database=skillswap;User=root;Password=Passw0rd!;";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(conn, ServerVersion.AutoDetect(conn)));

// Redis ConnectionMultiplexer FIRST
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConn = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";

    var cfg = ConfigurationOptions.Parse(redisConn);
    cfg.AbortOnConnectFail = false; // prevents crash

    return ConnectionMultiplexer.Connect(cfg);
});


// Redis Cache wrapper
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// services
builder.Services.AddScoped<SkillSwap.Api.Services.Skill.ISkillService,
                           SkillSwap.Api.Services.Skill.SkillService>();

builder.Services.AddScoped<SkillSwap.Api.Services.Location.ILocationCacheService,
                           SkillSwap.Api.Services.Location.RedisLocationCacheService>();

// Auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
