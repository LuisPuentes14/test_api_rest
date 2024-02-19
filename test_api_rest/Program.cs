using Microsoft.AspNetCore.Authentication.Cookies;
using test_api_rest.IOC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.InyectarDependencia(builder.Configuration);

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(option =>
//    {
//        option.LoginPath = "/PolarisServer/Salir";
//        option.Cookie.Name = "CookeAuthentication";
//        // option.ExpireTimeSpan = TimeSpan.FromMinutes(20); //tiempo expiración
//    });

builder.Services.AddDistributedMemoryCache(); // Necesario para el almacenamiento de la sesión

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8); // Tiempo de vida de la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

//app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
