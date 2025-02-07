using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TemplatePustokApp;
using TemplatePustokApp.Data;
using TemplatePustokApp.Models;
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
builder.Services.AddScoped<EmailService>();
builder.Services.AddSession();//vaxt verilmir ,brauzeri baglayanda melumatlar silinir
//builder.Services.AddSession(opt =>
//{
//    opt.IdleTimeout = TimeSpan.FromSeconds(20); aplication run da olsa bele 20 san sonra silinir
//});

builder.Services.AddIdentity<AppUser,IdentityRole>(opt=>
{
    opt.Password.RequireDigit = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireNonAlphanumeric = true;
    opt.Password.RequireUppercase = true;
    opt.Password.RequiredLength = 6;

    opt.User.RequireUniqueEmail = true;
    opt.SignIn.RequireConfirmedEmail = true;//login oan userin email confirm deyilse login ola bilmesin

    opt.Lockout.MaxFailedAccessAttempts = 3;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    opt.Lockout.AllowedForNewUsers = true;
}).AddEntityFrameworkStores<PustokAppDbContext>().AddDefaultTokenProviders();

var app = builder.Build();

app.UseSession();
app.UseStaticFiles();

app.UseAuthentication();//midlewear,userler ucun
app.UseAuthorization();//midlewear,rollar ucun

app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
       );

app.MapDefaultControllerRoute();

app.Run();
