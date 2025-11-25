using Microsoft.EntityFrameworkCore;
using SkillSwap.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var conn = builder.Configuration.GetConnectionString("DefaultConnection") 
           ?? "Server=127.0.0.1;Port=3306;Database=skillswap;User=root;Password=Passw0rd!;";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(conn, ServerVersion.AutoDetect(conn)));

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
