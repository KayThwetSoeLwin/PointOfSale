using Microsoft.EntityFrameworkCore;
using PointOfSale.Database.AppDbContextModels;
using Microsoft.AspNetCore.Http;
using FluentEmail.Smtp;
using FluentEmail.Core;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);

// Step 1: Add FluentEmail Configuration
string email = builder.Configuration["EmailSetting:Email"]!;
string emailAppPassword = builder.Configuration["EmailSetting:Password"]!;

builder.Services
    .AddFluentEmail(email)
    .AddSmtpSender(new SmtpClient("smtp.gmail.com")
    {
        Port = 587,
        Credentials = new System.Net.NetworkCredential(email, emailAppPassword),
        EnableSsl = true
    });

// Step 2: Register EmailService for Dependency Injection
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Services for Dependency Injection
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMenuPermissionService, MenuPermissionService>();
builder.Services.AddScoped<IStaffService, StaffService>();

// Add MVC Controllers with Views
builder.Services.AddControllersWithViews();

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Staff/Login";
        options.AccessDeniedPath = "/Staff/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });
builder.Services.Configure<PaginationConfig>(
    builder.Configuration.GetSection("AppSettings"));


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

// Register IHttpContextAccessor so it can be injected into services
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Staff}/{action=Login}/{id?}");

app.Run();
