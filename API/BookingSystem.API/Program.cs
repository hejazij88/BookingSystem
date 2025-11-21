using BookingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// خواندن ConnectionString از فایل تنظیمات
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ثبت DbContext در سیستم تزریق وابستگی (DI)
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(connectionString));

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

app.UseAuthorization();

app.MapControllers();

app.Run();
