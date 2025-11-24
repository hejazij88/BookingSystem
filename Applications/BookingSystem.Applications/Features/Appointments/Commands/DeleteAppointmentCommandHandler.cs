using BookingSystem.Applications.Features.Appointments.Commands.Records;
using BookingSystem.Applications.Hubs;
using BookingSystem.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace BookingSystem.Applications.Features.Appointments.Commands;

public class DeleteAppointmentCommandHandler : IRequestHandler<DeleteAppointmentCommand>
{
    private readonly BookingDbContext _context;

    private readonly IHubContext<AppointmentHub> _hubContext;


    public DeleteAppointmentCommandHandler(BookingDbContext context, IHubContext<AppointmentHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<Unit> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments.FindAsync(request.Id);
        if (appointment == null)
            throw new Exception("Appointment not found");

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync(cancellationToken);


        await _hubContext.Clients.All.SendAsync("ReceiveAppointmentDeleted", new
        {
            appointment.StartTime
        });

        return Unit.Value;
    }
}