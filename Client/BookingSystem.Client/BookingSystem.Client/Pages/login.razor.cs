using System.Net.Http.Json;
using BookingSystem.Client.Models;
using MudBlazor;

namespace BookingSystem.Client.Pages;

public partial class Login
{
    private readonly LoginModel model = new();
    private MudForm? _form;
    private bool _isSubmitting;

    private sealed class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }

    private async Task HandleLogin()
    {
        if (_form is null)
        {
            return;
        }

        await _form.Validate();
        if (!_form.IsValid)
        {
            Snackbar.Add("لطفاً ایمیل و رمز عبور را به درستی وارد کنید.", Severity.Warning);
            return;
        }

        _isSubmitting = true;

        try
        {
            var response = await Http.PostAsJsonAsync("api/Auth/login", model);

            if (!response.IsSuccessStatusCode)
            {
                Snackbar.Add("نام کاربری یا رمز عبور اشتباه است.", Severity.Error);
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result == null || string.IsNullOrWhiteSpace(result.Token))
            {
                Snackbar.Add("توکن معتبر دریافت نشد.", Severity.Error);
                return;
            }

            await LocalStorage.SetItemAsync("authToken", result.Token);
            await AuthStateProvider.GetAuthenticationStateAsync();

            Snackbar.Add("ورود موفقیت آمیز بود!", Severity.Success);
            NavManager.NavigateTo("/");
        }
        catch (Exception ex)
        {
            Snackbar.Add($"خطا در ارتباط با سرور: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isSubmitting = false;
        }
    }
}
