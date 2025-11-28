using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace BookingSystem.Client.Services;

public class AuthMessageHandler: DelegatingHandler
{
    private readonly AuthService _authService;

    private readonly IJSRuntime _js;

    public AuthMessageHandler(IJSRuntime js, AuthService authService)
    {
        _js = js;
        _authService = authService;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await _js.InvokeAsync<string>("localStorage.getItem", "accessToken");
        if (!string.IsNullOrEmpty(accessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var refreshed = await _authService.RefreshTokenAsync();
            if (refreshed)
            {
                accessToken = await _js.InvokeAsync<string>("localStorage.getItem", "accessToken");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                response = await base.SendAsync(request, cancellationToken);
            }
        }

        return response;
    }

}