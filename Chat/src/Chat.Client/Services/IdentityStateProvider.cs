using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Chat.Client.Services;

public class IdentityStateProvider: AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    //private readonly HttpClient _httpClient;
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
    
    public IdentityStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        //_httpClient = httpClient;
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var username = jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;

            var identity = new ClaimsIdentity(username != null ? 
                new[]
                {
                    new Claim(ClaimTypes.Name, username)
                }
                : Array.Empty<Claim>(), "jwt");

            _currentUser = new ClaimsPrincipal(identity);
        }

        return new AuthenticationState(_currentUser);
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
        
        if(!string.IsNullOrEmpty(username))
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "username", username);
        
        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, username ?? "")], "jwt");
        _currentUser = new ClaimsPrincipal(identity);
        
        //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    
    public async Task<string?> GetUser()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        
        if(string.IsNullOrEmpty(token))
            return null; 
        token = token.Replace("\"", "");
        if (string.IsNullOrEmpty(token)) return null;

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        return jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;
    }

    public async Task<string?> GetToken()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        return string.IsNullOrEmpty(token) ? null : token.Replace("\"", "");
    }
    
    public async Task<string?> GetUserId()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

        if (string.IsNullOrEmpty(token)) return null;

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token.Replace("\"", "")) as JwtSecurityToken;
        
        return jsonToken?.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
    }

    public async Task<bool> IsAuthenticated()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        if (string.IsNullOrEmpty(token)) return false;

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token.Replace("\"", "")) as JwtSecurityToken;
        
        return jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub") != null;
    }


    public async Task Logout()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "username");
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}