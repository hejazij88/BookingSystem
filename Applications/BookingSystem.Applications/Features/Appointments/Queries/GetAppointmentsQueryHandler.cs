using BookingSystem.Applications.DTOs;
using BookingSystem.Applications.Features.Appointments.Queries.Records;
using BookingSystem.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Applications.Features.Appointments.Queries;

public class GetAppointmentsQueryHandler:IRequestHandler<GetAppointmentsQuery,List<AppointmentDto>>
{
    private readonly BookingDbContext _context;

    public GetAppointmentsQueryHandler(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<List<AppointmentDto>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Appointments
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