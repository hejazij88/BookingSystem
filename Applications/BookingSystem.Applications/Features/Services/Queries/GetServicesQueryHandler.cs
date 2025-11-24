using BookingSystem.Applications.DTOs;
using BookingSystem.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Applications.Features.Services.Queries;

public class GetServicesQueryHandler : IRequestHandler<GetServicesQuery, List<ServiceDto>>
{
    private readonly BookingDbContext _context;

    public GetServicesQueryHandler(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<List<ServiceDto>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
    {
        // منطق اصلی: کوئری به دیتابیس و تبدیل به DTO
        var services = await _context.Services
            .Select(s => new ServiceDto
            {
                Id = s.Id,
                Name = s.Name,
                DurationMinutes = s.DurationMinutes,
                Price = s.Price
            })
            .ToListAsync(cancellationToken);

        return services;
    }
}