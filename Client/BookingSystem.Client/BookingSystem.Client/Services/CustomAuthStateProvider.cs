using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace BookingSystem.Client.Services
{
    public class CustomAuthStateProvider: AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;

        public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient http)
        {
            _localStorage = localStorage;
            _http = http;
        }

        // این متد هر بار که صفحه رفرش شود یا وضعیت تغییر کند اجرا می‌شود
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // 1. دریافت توکن از حافظه
            string token = await _localStorage.GetItemAsStringAsync("authToken");

            var identity = new ClaimsIdentity(); // یعنی کاربر ناشناس است
            _http.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    // 2. خواندن اطلاعات توکن
                    var claims = JwtParser.ParseClaimsFromJwt(token);

                    // 3. ساخت هویت کاربر
                    identity = new ClaimsIdentity(claims, "jwt");

                    // 4. چسباندن توکن به تمام درخواست‌های بعدی HTTP
                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                catch
                {
                    // اگر توکن خراب بود، آن را پاک کن
                    await _localStorage.RemoveItemAsync("authToken");
                }
            }

            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);

            // به کامپوننت‌ها خبر بده که وضعیت تغییر کرد
            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return state;
        }


    }
}
