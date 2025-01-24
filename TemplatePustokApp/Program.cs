using Microsoft.EntityFrameworkCore;
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

var app = builder.Build();

app.UseStaticFiles();

app.MapDefaultControllerRoute();

app.Run();
