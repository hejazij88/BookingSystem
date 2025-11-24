using System.Net.Http.Json;
using BookingSystem.Client.Models;

namespace BookingSystem.Client.Services;

public class PaymentApiService
{
    private readonly HttpClient _httpClient;

    public PaymentApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PaymentResponseModel?> PayForAppointmentAsync(int appointmentId)
    {
        var response = await _httpClient.PostAsync($"api/payments/{appointmentId}", null);
        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync();
            return new PaymentResponseModel
            {
                Success = false,
                Message = string.IsNullOrWhiteSpace(message) ? "پرداخت ناموفق بود." : message
            };
        }

        return await response.Content.ReadFromJsonAsync<PaymentResponseModel>();
    }
}

