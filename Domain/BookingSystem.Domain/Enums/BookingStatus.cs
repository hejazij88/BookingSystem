namespace BookingSystem.Domain.Enums;

public enum BookingStatus
{
    Pending = 1,    // در انتظار پرداخت یا تایید
    Confirmed = 2,  // قطعی شده
    Cancelled = 3,  // لغو شده
    Completed = 4   // انجام شده (بعد از ارائه خدمت)
}