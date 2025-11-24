using System.Net.Http.Json;
using BookingSystem.Client.HubsConnection;
using BookingSystem.Client.Models;
using BookingSystem.Client.Services;
using BookingSystem.Domain.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using AppointmentRealtimeDto = BookingSystem.Applications.DTOs.AppointmentRealtimeDto;

namespace BookingSystem.Client.Pages;

public partial class Book : IDisposable
{
    private List<ServiceDto> Services = new();
    private List<TimeSlotDto> Slots = new();
    private ServiceDto? SelectedService { get; set; }
    private DateTime? SelectedDate { get; set; } = DateTime.Today.AddDays(1); // پیش فرض: فردا
    private bool IsLoading = false;
    private bool _realtimeSubscribed;
    private bool _payOnline = true;
    private bool _isBooking;

    [Inject] private AppointmentSignalRService AppointmentSignalRService { get; set; } = default!;
    [Inject] private PaymentApiService PaymentService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {

        SubscribeToRealtimeEvents();
        await EnsureRealtimeConnection();
        await LoadServices();
        await LoadAvailableSlots();
    }

    private void SubscribeToRealtimeEvents()
    {
        if (_realtimeSubscribed)
        {
            return;
        }

        AppointmentSignalRService.OnAppointmentCreated += HandleAppointmentChanged;
        AppointmentSignalRService.OnAppointmentUpdated += HandleAppointmentChanged;
        AppointmentSignalRService.OnAppointmentDeleted += HandleAppointmentChanged;
        _realtimeSubscribed = true;
    }

    private void UnsubscribeFromRealtimeEvents()
    {
        if (!_realtimeSubscribed)
        {
            return;
        }

        AppointmentSignalRService.OnAppointmentCreated -= HandleAppointmentChanged;
        AppointmentSignalRService.OnAppointmentUpdated -= HandleAppointmentChanged;
        AppointmentSignalRService.OnAppointmentDeleted -= HandleAppointmentChanged;
        _realtimeSubscribed = false;
    }

    private async Task EnsureRealtimeConnection()
    {
        try
        {
            await AppointmentSignalRService.EnsureConnectedAsync(NavManager);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"اتصال بلادرنگ برقرار نشد: {ex.Message}", Severity.Warning);
        }
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

        if (_isBooking)
        {
            return;
        }

        _isBooking = true;

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
                var result = await response.Content.ReadFromJsonAsync<CreateAppointmentResponse>();
                Snackbar.Add("✅ نوبت شما با موفقیت ثبت شد!", Severity.Success);

                if (_payOnline && result != null)
                {
                    await ProcessPaymentAsync(result.AppointmentId);
                }

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
        finally
        {
            _isBooking = false;
        }
    }

    private async Task ProcessPaymentAsync(int appointmentId)
    {
        var paymentResult = await PaymentService.PayForAppointmentAsync(appointmentId);
        if (paymentResult == null)
        {
            Snackbar.Add("پرداخت انجام نشد. لطفاً بعداً امتحان کنید.", Severity.Warning);
            return;
        }

        if (paymentResult.Success)
        {
            Snackbar.Add($"پرداخت موفق بود. کد رهگیری: {paymentResult.Reference}", Severity.Success);
        }
        else
        {
            Snackbar.Add($"پرداخت ناموفق بود: {paymentResult.Message}", Severity.Warning);
        }
    }

    private void HandleAppointmentChanged(AppointmentRealtimeDto dto)
    {
        if (!ShouldRefreshSlots(dto))
        {
            return;
        }

        _ = InvokeAsync(async () =>
        {
            await LoadAvailableSlots();

            var message = dto.Status switch
            {
                BookingStatus.Cancelled => "یک نوبت آزاد شد. لیست سانس‌ها بروزرسانی شد.",
                BookingStatus.Confirmed => "یک نوبت تایید شد. لیست سانس‌ها بروزرسانی شد.",
                _ => "نوبت دیگری تغییر کرد. لیست سانس‌ها بروزرسانی شد."
            };

            Snackbar.Add(message, Severity.Info);
        });
    }

    private bool ShouldRefreshSlots(AppointmentRealtimeDto dto)
    {
        if (SelectedService == null || !SelectedDate.HasValue)
        {
            return false;
        }

        return dto.ServiceId == SelectedService.Id &&
               dto.StartTime.Date == SelectedDate.Value.Date;
    }

    public void Dispose()
    {
        UnsubscribeFromRealtimeEvents();
    }
}