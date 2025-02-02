using AirportDemo.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// **Session desteğini ekle**
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 30 dakika boyunca oturum açık kalır
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Kullanıcı oturum verilerine erişim için gerekli
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// **Session middleware'i etkinleştir**
app.UseSession();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Kullanıcı oturum kontrolü (Login olmadan erişimi engellemek için)
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    // Eğer kullanıcı giriş yapmamışsa ve bu sayfalara erişmek istiyorsa yönlendir
    if (string.IsNullOrEmpty(context.Session.GetString("UserId")) &&
        (path.StartsWith("/Flights") || path.StartsWith("/Fuel") || path.StartsWith("/Maintenance") || path.StartsWith("/Logistics")) &&
        !path.StartsWith("/Auth/Login") &&
        !path.StartsWith("/Auth/Register") &&
        !path.StartsWith("/css") &&
        !path.StartsWith("/js") &&
        !path.StartsWith("/images"))
    {
        context.Response.Redirect("/Auth/Login");
        return;
    }

    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
