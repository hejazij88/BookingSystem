using BookingSystem.Applications.DTOs;
using BookingSystem.Domain.Models;
using BookingSystem.Infrastructure.Data;
using MediatR;

namespace BookingSystem.Applications.Features.Services.Commands;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, ServiceDto>
{
    private readonly BookingDbContext _context;

    public CreateServiceCommandHandler(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceDto> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        // منطق اصلی: تبدیل DTO به Entity و ذخیره در دیتابیس
        var service = new Service
        {
            Name = request.Name,
            DurationMinutes = request.DurationMinutes,
            Price = request.Price,
            IsActive = true
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync(cancellationToken);

        // تبدیل Entity خروجی به DTO و برگرداندن
        return new ServiceDto
        {
            Id = service.Id,
            Name = service.Name,
            DurationMinutes = service.DurationMinutes,
            Price = service.Price
        };
    }
}