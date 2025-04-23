using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace Chat.Client.Services;

public class AuthHeaderService: DelegatingHandler
{
    private readonly IdentityStateProvider _identityStateProvider;
    private readonly NavigationManager _navigationManager;
    public AuthHeaderService(IdentityStateProvider identityStateProvider, NavigationManager navigationManager)
    {
        _identityStateProvider = identityStateProvider;
        _navigationManager = navigationManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _identityStateProvider.GetToken();
        
        if(!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);

        if(response.StatusCode == HttpStatusCode.Unauthorized)
            _navigationManager.NavigateTo("/");
        
        return response;
    }
}