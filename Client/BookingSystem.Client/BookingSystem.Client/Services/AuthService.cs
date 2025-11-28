using BookingSystem.Applications.DTOs.ResponseDTOs;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BookingSystem.Client.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public AuthService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", new { Email = email, Password = password });
        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        await _js.InvokeVoidAsync("localStorage.setItem", "accessToken", result.AccessToken);
        await _js.InvokeVoidAsync("localStorage.setItem", "refreshToken", result.RefreshToken);

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
        return true;
    }

    public async Task<bool> RefreshTokenAsync()
    {
        var refreshToken = await _js.InvokeAsync<string>("localStorage.getItem", "refreshToken");
        if (string.IsNullOrEmpty(refreshToken)) return false;

        var response = await _http.PostAsJsonAsync("api/auth/refresh", new { Token = refreshToken });
        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        await _js.InvokeVoidAsync("localStorage.setItem", "accessToken", result.AccessToken);
        await _js.InvokeVoidAsync("localStorage.setItem", "refreshToken", result.RefreshToken);

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
        return true;
    }
}