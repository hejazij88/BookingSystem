using System.Net.Http.Json;
using BookingSystem.Client.Models;
using MudBlazor;

namespace BookingSystem.Client.Pages;

public partial class login
{
    private LoginModel model = new LoginModel();

    private class LoginResponse
    {
        public string Token { get; set; }
    }

    private async Task HandleLogin()
    {
        try
        {
            // ارسال درخواست به API
            var response = await Http.PostAsJsonAsync("api/Auth/login", model);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                // ذخیره توکن در مرورگر
                await LocalStorage.SetItemAsync("authToken", result.Token);

                await AuthStateProvider.GetAuthenticationStateAsync();

                Snackbar.Add("ورود موفقیت آمیز بود!", Severity.Success);
                NavManager.NavigateTo("/"); // هدایت به صفحه اصلی
            }
            else
            {
                Snackbar.Add("نام کاربری یا رمز عبور اشتباه است.", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"خطا در ارتباط با سرور: {ex.Message}", Severity.Error);
        }
    }
}