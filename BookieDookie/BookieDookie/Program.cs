using BookieDookie.Data;
using BookieDookie.Models;
using BookieDookie.Services;
using BookieDookie.Services.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Hamro pyaro MVC

builder.Services.AddControllersWithViews();

//Session thingy

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//Biwi ki DB

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));

//The strict DI

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookService, BookService>();

//The first A: Authentication

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";

        // Users who fail an authorization requirement are redirected here.
        options.AccessDeniedPath = "/Profile/AccessDenied";

        options.ExpireTimeSpan =
            TimeSpan.FromHours(2);

        options.SlidingExpiration = true;

        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

//The second A: Authorization

builder.Services.AddAuthorization();

//Let's build the appieeeee now:)

var app = builder.Build();

//Siberian birds migrate now!!!

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

//URL de, Bhai

app.Urls.Add("http://0.0.0.0:5026");

//HTTP

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


//Hami hinne default baato

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");


app.Run();