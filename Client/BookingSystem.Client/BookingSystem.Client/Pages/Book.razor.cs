using System.Net.Http.Json;
using BookingSystem.Client.Models;
using MudBlazor;

namespace BookingSystem.Client.Pages;

public partial class Book
{
    private List<ServiceDto> Services = new();
    private List<TimeSlotDto> Slots = new();
    private ServiceDto? SelectedService { get; set; }
    private DateTime? SelectedDate { get; set; } = DateTime.Today.AddDays(1); // پیش فرض: فردا
    private bool IsLoading = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadServices();
        await LoadAvailableSlots();
    }

    private async Task LoadServices()
    {
        try
        {
            Services = await Http.GetFromJsonAsync<List<ServiceDto>>("api/services") ?? new List<ServiceDto>();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"خطا در بارگذاری سرویس‌ها: {ex.Message}", Severity.Error);
        }
    }

    private async Task OnSelectionChanged()
    {
        // وقتی سرویس یا تاریخ عوض شد، سانس‌های جدید را لود کن
        if (SelectedService != null && SelectedDate.HasValue)
        {
            await LoadAvailableSlots();
        }
        else
        {
            Slots = new(); // خالی کن
        }
    }

    private async Task LoadAvailableSlots()
    {
        if (SelectedService == null || !SelectedDate.HasValue) return;

        IsLoading = true;
        Slots = new();

        try
        {
            // فراخوانی منطق تولید سانس‌های خالی از API
            var dateParam = SelectedDate.Value.ToString("yyyy-MM-dd");
            var url = $"api/services/{SelectedService.Id}/slots?date={dateParam}";
            Slots = await Http.GetFromJsonAsync<List<TimeSlotDto>>(url) ?? new List<TimeSlotDto>();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"خطا در دریافت زمان‌های خالی: {ex.Message}", Severity.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task BookSlot(TimeSlotDto slot)
    {
        if (SelectedService == null) return;

        var appointment = new CreateAppointmentDto
        {
            ServiceId = SelectedService.Id,
            StartTime = slot.StartTime,
            Note = "" // فعلا یادداشت خالی
        };

        try
        {
            // ارسال درخواست POST به AppointmentsController
            var response = await Http.PostAsJsonAsync("api/appointments", appointment);

            if (response.IsSuccessStatusCode)
            {
                Snackbar.Add("✅ نوبت شما با موفقیت ثبت شد!", Severity.Success);
                // رفرش لیست سانس‌ها (تا سانسی که رزرو شد، پاک شود)
                await LoadAvailableSlots();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Snackbar.Add($"⚠️ متاسفانه نوبت ثبت نشد: {errorContent}", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"خطای سیستمی: {ex.Message}", Severity.Error);
        }
    }
}