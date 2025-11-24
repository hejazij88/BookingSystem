using BookingSystem.Applications.Features.Appointments.Commands.Records;
using BookingSystem.Infrastructure.Data;
using MediatR;

namespace BookingSystem.Applications.Features.Appointments.Commands;

public class DeleteAppointmentCommandHandler : IRequestHandler<DeleteAppointmentCommand>
{
    private readonly BookingDbContext _context;

    public DeleteAppointmentCommandHandler(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments.FindAsync(request.Id);
        if (appointment == null)
            throw new Exception("Appointment not found");

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}