using BookingSystem.Applications.DTOs;
using BookingSystem.Applications.Features.Appointments.Commands.Records;
using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Models;
using BookingSystem.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Applications.Features.Appointments.Commands;

public class CreateAppointmentCommandHandler: IRequestHandler<CreateAppointmentCommand, int>
{
    private readonly BookingDbContext _context;

    public CreateAppointmentCommandHandler(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        // 1. سرویس وجود دارد؟
        var service = await _context.Services.FindAsync(request.Dto.ServiceId);
        if (service == null) throw new Exception("سرویس مورد نظر یافت نشد.");

        // 2. محاسبه زمان پایان
        var endTime = request.Dto.StartTime.AddMinutes(service.DurationMinutes);

        // 3. بررسی Double Booking
        bool conflict = await _context.Appointments.AnyAsync(a =>
            a.Status != BookingStatus.Cancelled &&
            a.StartTime < endTime &&
            a.EndTime > request.Dto.StartTime, cancellationToken);

        if (conflict)
            throw new Exception("متاسفانه این زمان لحظاتی پیش رزرو شد. لطفاً زمان دیگری را انتخاب کنید.");

        // 4. ساخت موجودیت
        var appointment = new Appointment
        {
            ServiceId = request.Dto.ServiceId,
            UserId = request.UserId,
            StartTime = request.Dto.StartTime,
            EndTime = endTime,
            Status = BookingStatus.Pending,
            Note = request.Dto.Note,
            CreatedAt = DateTime.UtcNow
        };

        // 5. ذخیره
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync(cancellationToken);

        return appointment.Id;
    }
}