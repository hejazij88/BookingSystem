using BookingSystem.Applications.DTOs;
using BookingSystem.Domain.Enums;
using MudBlazor;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BookingSystem.Client.Pages.User;

public partial class PanelUser
{
    private HttpClient Http => ClientFactory.CreateClient("API");
    private int activeTab = 0;
    private List<AppointmentDto>? ActiveBookings = new List<AppointmentDto>();
    private List<AppointmentDto>? BookingHistory = new List<AppointmentDto>();

 

    protected override async Task OnInitializedAsync()
    {

        try
        {
            ActiveBookings = await Http.GetFromJsonAsync<List<AppointmentDto>>(
                "api/Appointments/GetAppointmentsByUserId?isActive=true");
        }
        catch (Exception ex)
        {
            Snackbar.Add($"خطا در بارگذاری: {ex.Message}", Severity.Error);
            ActiveBookings = new List<AppointmentDto>();
        }

        try
        {
            BookingHistory = await Http.GetFromJsonAsync<List<AppointmentDto>>(
                "api/Appointments/GetAppointmentsByUserId?isActive=false"); 
        }
        catch (Exception ex)
        {
            Snackbar.Add($"خطا در بارگذاری: {ex.Message}", Severity.Error);
            BookingHistory = new List<AppointmentDto>();
        }
    }

    private void ViewBooking(int id)
    {
        NavManager.NavigateTo($"/user/bookings/{id}");
    }

    private void EditBooking(int id)
    {
        NavManager.NavigateTo($"/user/bookings/edit/{id}");
    }

    private void CancelBooking(int id)
    {
        // ⚡ اینجا می‌توانید درخواست API برای لغو رزرو ارسال کنید
        var booking = ActiveBookings.FirstOrDefault(b => b.Id == id);
        if (booking != null)
        {
            ActiveBookings.Remove(booking);
            BookingHistory.Add(booking); // انتقال به تاریخچه با وضعیت لغو شده
            booking.Status = BookingStatus.Cancelled;
        }
    }

}