using Microsoft.EntityFrameworkCore;
using TemplatePustokApp;
using TemplatePustokApp.Data;
using TemplatePustokApp.Services;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<PustokAppDbContext>(options =>
{
    options.UseSqlServer(config["ConnectionStrings:DefaultConnection"]);
});

builder.Services.AddScoped<LayoutServices>();
builder.Services.Configure<JwtServiceOption>(config.GetSection("Jwt"));

var app = builder.Build();

app.UseStaticFiles();

app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
       );

app.MapDefaultControllerRoute();

app.Run();
