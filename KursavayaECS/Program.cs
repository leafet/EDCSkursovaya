using Microsoft.EntityFrameworkCore;
using KursavayaECS.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using KursavayaECS.Models;
using KursavayaECS.AppServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuthenticationServices, AutenticationServices>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var config = builder.Configuration;

var connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("NO string");
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("SomeSecretSheeshAndShushAndShashAndManyOtherSecretThingsAlreadyToComplicated"))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["tasty-kokis"];

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireClaim("Admin", "true");
    });

    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.RequireClaim("Default", "true");
    });

    options.AddPolicy("StudentPolicy", policy =>
    {
        policy.RequireClaim("Student", "true");
    });

    options.AddPolicy("TeacherPolicy", policy =>
    {
        policy.RequireClaim("Teacher", "true");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    Secure = CookieSecurePolicy.Always
});

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
