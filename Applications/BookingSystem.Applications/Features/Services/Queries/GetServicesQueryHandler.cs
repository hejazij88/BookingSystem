using BookingSystem.Applications.DTOs;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Applications.Features.Services.Queries;

public class GetServicesQueryHandler : IRequestHandler<GetServicesQuery, List<ServiceDto>>
{
    private readonly IServiceRepository _serviceRepository;

    public GetServicesQueryHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<List<ServiceDto>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
    {
        // منطق: استفاده از متد تعریف شده در Repository
        var services = await _serviceRepository.GetAllServicesAsync();

        // ... بقیه منطق تبدیل به DTO ...
        // چون GetAllServicesAsync موجودیت‌ها را برمی‌گرداند، باید اینجا تبدیل به DTO شود
        return services.Select(s => new ServiceDto
        {
            Id = s.Id,
            Name = s.Name,
            DurationMinutes = s.DurationMinutes,
            Price = s.Price
        }).ToList();
    }
}