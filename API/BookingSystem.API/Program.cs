using BookingSystem.Applications.Behaviors;
using BookingSystem.Applications.Features.Services.Queries;
using BookingSystem.Applications.Hubs;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Domain.Models;
using BookingSystem.Infrastructure.Data;
using BookingSystem.Infrastructure.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMediatR(typeof(GetServicesQuery).Assembly);

builder.Services.AddValidatorsFromAssembly(typeof(GetServicesQuery).Assembly);


builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


builder.Services.AddScoped<IServiceRepository, ServiceRepository>();

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

builder.Services.AddSignalR();


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

app.MapHub<AppointmentHub>("/appointmentHub");


app.UseCors("AllowBlazorOrigin");

app.UseAuthentication(); // اول: کیستی؟
app.UseAuthorization();  // دوم: اجازه داری؟
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // اجرای متد Seed (برای ساخت نقش‌ها و کاربر ادمین)
        await IdentityInitializer.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
