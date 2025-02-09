using AirportDemo.Data;
using AirportDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// **Database bağlantısını ekleyelim**
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// **Identity Konfigürasyonu (ÖNCE Identity Yapılandırmasını Ekle)**
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // E-posta doğrulaması devre dışı
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// **Sonra Cookie Yapılandırmasını Ekle**
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login"; // Kullanıcı giriş yapmamışsa bu sayfaya yönlendirilir
    options.AccessDeniedPath = "/Auth/AccessDenied"; // Yetkisiz erişim için yönlendirme
    options.LogoutPath = "/Auth/Logout"; // Çıkış sayfası
    options.Cookie.HttpOnly = true; // Güvenlik için
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Oturum süresi
    options.SlidingExpiration = true; // Süre uzatma
});

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient(); // Register HttpClientFactory
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
