using System.Text;
using BookingSystem.Domain.Models;
using BookingSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorOrigin",
        builder => builder
            .AllowAnyOrigin() // در محیط واقعی باید آدرس دقیق سایت را بدهید
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// خواندن ConnectionString از فایل تنظیمات
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ثبت DbContext در سیستم تزریق وابستگی (DI)
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false; // برای سادگی تست، پسورد ساده قبول کند
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<BookingDbContext>()
    .AddDefaultTokenProviders();

// 3. تنظیمات احراز هویت با JWT
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!))
        };
    });


builder.Services.AddScoped<BookingSystem.API.Services.AvailabilityService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorOrigin");

app.UseAuthentication(); // اول: کیستی؟
app.UseAuthorization();  // دوم: اجازه داری؟
app.MapControllers();

app.Run();
