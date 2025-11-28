using Blazored.LocalStorage;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BookingSystem.Client.Services;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorageService;
    private readonly IJSRuntime _js;
    private readonly AuthService _authService;

    public AuthMessageHandler(IJSRuntime js, AuthService authService, ILocalStorageService localStorageService)
    {
        _js = js;
        _authService = authService;
        _localStorageService = localStorageService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // دریافت Access Token صحیح از LocalStorage
        //var accessToken = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
        var accessToken = await _localStorageService.GetItemAsStringAsync("authToken");


        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // اگر توکن منقضی شده باشد → Unauthorized می‌دهد
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var refreshed = await _authService.RefreshTokenAsync();

            if (refreshed)
            {
                accessToken = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");

                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                response = await base.SendAsync(request, cancellationToken);
            }
        }

        return response;
    }
}






public interface IApiService
{
    Task<T> GetAsync<T>(string url);
    Task<T> PostAsync<T>(string url, object data);
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorageService;

    public ApiService(HttpClient httpClient, ILocalStorageService localStorageService)
    {
        _httpClient = httpClient;
        _localStorageService = localStorageService;
    }

    private async Task AddAuthHeader()
    {
        var token = await _localStorageService.GetItemAsStringAsync("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

    }

    public async Task<T> GetAsync<T>(string url)
    {
        await AddAuthHeader();
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T> PostAsync<T>(string url, object data)
    {
        await AddAuthHeader();
        var response = await _httpClient.PostAsJsonAsync(url, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }
}