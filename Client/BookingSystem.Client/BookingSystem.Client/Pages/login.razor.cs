using System.Net.Http.Headers;
using BookingSystem.Client.Models;
using BookingSystem.Client.Services;
using MudBlazor;
using System.Net.Http.Json;

namespace BookingSystem.Client.Pages;

public partial class Login
{
    private HttpClient Http;
    private readonly LoginModel model = new();
    private MudForm? _form;
    private bool _isSubmitting;

    public Login(HttpClient http)
    {
        Http = http;
    }

    protected override void OnInitialized()
    {
        Http = ClientFactory.CreateClient("API");
    }

    private sealed class LoginResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }

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
            if (result == null || string.IsNullOrWhiteSpace(result.AccessToken))
            {
                Snackbar.Add("توکن معتبر دریافت نشد.", Severity.Error);
                return;
            }

            await LocalStorage.SetItemAsync("AccessToken", result.AccessToken);
            await LocalStorage.SetItemAsync("refreshToken", result.RefreshToken);



            await AuthStateProvider.GetAuthenticationStateAsync();

            //Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken); 

            StateHasChanged();

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
