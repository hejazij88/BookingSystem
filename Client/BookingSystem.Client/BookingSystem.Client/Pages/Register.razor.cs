using System.Net.Http.Json;
using BookingSystem.Client.Models;
using MudBlazor;

namespace BookingSystem.Client.Pages;

public partial class Register
{
    private readonly RegisterModel _model = new();
    private MudForm? _form;
    private bool _isSubmitting;

    private async Task HandleRegister()
    {
        if (_form is null)
        {
            return;
        }

        await _form.Validate();
        if (!_form.IsValid)
        {
            Snackbar.Add("لطفاً خطاهای فرم را برطرف کنید.", Severity.Warning);
            return;
        }

        _isSubmitting = true;

        try
        {
            var registerPayload = new
            {
                _model.FirstName,
                _model.LastName,
                _model.Email,
                _model.Password
            };

            var response = await Http.PostAsJsonAsync("api/Auth", registerPayload);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Snackbar.Add($"ثبت‌نام با خطا مواجه شد: {error}", Severity.Error);
                return;
            }

            Snackbar.Add("ثبت‌نام با موفقیت انجام شد. ورود خودکار در حال انجام است...", Severity.Success);

            await SignInAfterRegister();
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

    private async Task SignInAfterRegister()
    {
        var loginPayload = new LoginModel
        {
            Email = _model.Email,
            Password = _model.Password
        };

        var response = await Http.PostAsJsonAsync("api/Auth/login", loginPayload);
        if (!response.IsSuccessStatusCode)
        {
            Snackbar.Add("ثبت‌نام انجام شد اما ورود خودکار ناموفق بود. لطفاً به صورت دستی وارد شوید.", Severity.Warning);
            NavManager.NavigateTo("/login");
            return;
        }

        var loginResult = await response.Content.ReadFromJsonAsync<LoginResponse>();
        if (loginResult == null || string.IsNullOrWhiteSpace(loginResult.Token))
        {
            Snackbar.Add("توکن ورود دریافت نشد. لطفاً دوباره وارد شوید.", Severity.Warning);
            NavManager.NavigateTo("/login");
            return;
        }

        await LocalStorage.SetItemAsync("authToken", loginResult.Token);
        await AuthStateProvider.GetAuthenticationStateAsync();

        Snackbar.Add("به سیستم خوش آمدید!", Severity.Success);
        NavManager.NavigateTo("/");
    }

    private sealed class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}

