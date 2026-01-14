using CustomerManagementSystem.DbModels;
using CustomerManagementSystem.Infrastructure;
using CustomerManagementSystem.Services;
using CustomerManagementSystem.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using System.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


builder.Services.AddScoped<DapperContext>();




builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = builder.Configuration["Authentication:Cookie:LoginPath"];
        options.LogoutPath = builder.Configuration["Authentication:Cookie:LogoutPath"];
        options.AccessDeniedPath = builder.Configuration["Authentication:Cookie:AccessDeniedPath"];
        options.ExpireTimeSpan = TimeSpan.FromMinutes(
            int.Parse(builder.Configuration["Authentication:Cookie:ExpireMinutes"] ?? "0")
        );
        options.SlidingExpiration = bool.Parse(
            builder.Configuration["Authentication:Cookie:SlidingExpiration"] ?? "0");
    });


builder.Services.AddAuthorization();

var app = builder.Build();



if (!app.Environment.IsDevelopment())
{

    app.UseHsts();
}
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Customers}/{action=Index}/{id?}");

await app.RunAsync();
