using CustomerManagementSystem.DbModels;
using CustomerManagementSystem.Infrastructure;
using CustomerManagementSystem.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using System.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddSingleton<DapperContext>();

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
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Customers}/{action=Index}/{id?}");

app.Run();
