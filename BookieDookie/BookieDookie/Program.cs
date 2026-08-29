using BookieDookie.Data;
using BookieDookie.Models;
using BookieDookie.Services;
using BookieDookie.Services.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// MVC
// ==========================================

builder.Services.AddControllersWithViews();


// ==========================================
// SESSION
// ==========================================

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// ==========================================
// DATABASE
// ==========================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));


// ==========================================
// DEPENDENCY INJECTION
// ==========================================

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookService, BookService>();


// ==========================================
// AUTHENTICATION
// ==========================================

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";

        // Users who fail an authorization requirement
        // are redirected here.
        options.AccessDeniedPath = "/Profile/AccessDenied";

        options.ExpireTimeSpan =
            TimeSpan.FromHours(2);

        options.SlidingExpiration = true;

        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });


// ==========================================
// AUTHORIZATION
// ==========================================

builder.Services.AddAuthorization();


// ==========================================
// BUILD APP
// ==========================================

var app = builder.Build();

// ==========================================
// DATABASE MIGRATION
// ==========================================

using (var scope = app.Services.CreateScope())
{
    var db =
        scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

    db.Database.Migrate();
}


//Admin Seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    var passwordHasher = new PasswordHasher<User>();

    var adminUsername = "Admin";
    var adminEmail = "admin@bookiedookie.com";

    var existingAdmin = context.Users
        .FirstOrDefault(u =>
            u.Username == adminUsername);

    if (existingAdmin == null)
    {
        var admin = new User
        {
            Id = Guid.NewGuid(),

            Username = adminUsername,

            Email = adminEmail,

            Role = UserRole.Admin,

            Status = UserStatus.Active,

            IsDeleted = false
        };

        admin.PasswordHash =
            passwordHasher.HashPassword(
                admin,
                "Admin@123");

        context.Users.Add(admin);

        context.SaveChanges();
    }
}

// ==========================================
// SERVER URL
// ==========================================

app.Urls.Add("http://0.0.0.0:5026");


// ==========================================
// HTTP PIPELINE
// ==========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


// Session must come before controllers
app.UseSession();


// Authentication must come before Authorization
app.UseAuthentication();

app.UseAuthorization();


// ==========================================
// DEFAULT ROUTE
// ==========================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");


app.Run();