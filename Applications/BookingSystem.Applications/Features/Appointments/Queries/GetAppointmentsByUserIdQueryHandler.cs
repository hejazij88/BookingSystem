using BookingSystem.Applications.DTOs;
using BookingSystem.Applications.Features.Appointments.Queries.Records;
using BookingSystem.Domain.Enums;
using BookingSystem.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Applications.Features.Appointments.Queries;

public class GetAppointmentsByUserIdQueryHandler:IRequestHandler<GetAppointmentsByUserIdQuery,List<AppointmentDto>>
{
    private readonly BookingDbContext _context;

    public GetAppointmentsByUserIdQueryHandler(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<List<AppointmentDto>> Handle(GetAppointmentsByUserIdQuery request, CancellationToken cancellationToken)
    {
        if (!request.isActive)
        {
            return await _context.Appointments
                .Where(a=>a.UserId==request.Id.ToString()&& a.Status == BookingStatus.Completed && a.Status == BookingStatus.Cancelled)
                .Include(a => a.User)
                .Include(a=>a.Service)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    UserName = a.User.UserName,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Description = a.Note,
                    Status = a.Status,
                    ServiceTitle = a.Service.Name
                }).ToListAsync(cancellationToken);
        }
        else
        {
            return await _context.Appointments
                .Where(a => a.UserId == request.Id && a.Status == BookingStatus.Confirmed && a.Status == BookingStatus.Pending)
                .Include(a => a.User)
                .Include(a=>a.Service)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    UserName = a.User.UserName,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Description = a.Note,
                    Status = a.Status,
                    ServiceTitle = a.Service.Name
                }).ToListAsync(cancellationToken);
        }

       
    }
}