using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Client.Services;

public class AuthStateProvider: AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private readonly HttpClient _httpClient;
    private ClaimsPrincipal _user = new(new ClaimsIdentity());

    public AuthStateProvider(IJSRuntime jsRuntime, HttpClient httpClient)
    {
        _jsRuntime = jsRuntime;
        _httpClient = httpClient;
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var username = jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;

            var identity = new ClaimsIdentity(username != null
                ? new[] { new Claim(ClaimTypes.Name, username) }
                : Array.Empty<Claim>(), "jwt");

            _user = new ClaimsPrincipal(identity);
        }

        return new AuthenticationState(_user);
    }

    public async Task SetUser(string token)
    {
        if (string.IsNullOrEmpty(token))
            return;
        
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
        
        token = token.Replace("\"", "");
        
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        var username = jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;

        if (!string.IsNullOrEmpty(username))
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userName", username);
        }

        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, username ?? "")], "jwt");
        _user = new ClaimsPrincipal(identity);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        
    }
    
    public async Task<string?> GetUserName()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        
        token = token.Replace("\"", "");
        if (string.IsNullOrEmpty(token)) return null;

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        return jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;
    }
    
    public async Task Logout()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        _user = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}