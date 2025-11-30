using Blazored.LocalStorage;
using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace BookingSystem.Client.Services;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorageService;

    public AuthMessageHandler(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var accessToken = await _localStorageService.GetItemAsync<string>("AccessToken", cancellationToken);


        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        return response;
    }
}