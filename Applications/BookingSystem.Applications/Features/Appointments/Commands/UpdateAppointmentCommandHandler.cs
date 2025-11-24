using BookingSystem.Applications.Features.Appointments.Commands.Records;
using BookingSystem.Domain.Enums;
using BookingSystem.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Applications.Features.Appointments.Commands;

public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, int>
{
    private readonly BookingDbContext _context;

    public UpdateAppointmentCommandHandler(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        // پیدا کردن رزرو
        var appointment = await _context.Appointments
            .Include(a => a.Service)
            .FirstOrDefaultAsync(a => a.Id == request.Dto.Id && a.UserId == request.UserId, cancellationToken);

        if (appointment == null)
            throw new Exception("رزرو یافت نشد یا به شما تعلق ندارد.");

        // محاسبه زمان پایان جدید
        var endTime = request.Dto.StartTime.AddMinutes(appointment.Service.DurationMinutes);

        // بررسی تداخل زمانی با رزروهای دیگر
        bool conflict = await _context.Appointments.AnyAsync(a =>
                a.Id != appointment.Id &&
                a.Status != BookingStatus.Cancelled &&
                a.StartTime < endTime &&
                a.EndTime > request.Dto.StartTime,
            cancellationToken);

        if (conflict)
            throw new Exception("زمان جدید با رزروهای دیگر تداخل دارد. لطفاً زمان دیگری انتخاب کنید.");

        // اعمال تغییرات
        appointment.StartTime = request.Dto.StartTime;
        appointment.EndTime = endTime;
        appointment.Note = request.Dto.Description;

        await _context.SaveChangesAsync(cancellationToken);

        return appointment.Id;
    }
}