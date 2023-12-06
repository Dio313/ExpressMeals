using Microsoft.AspNetCore.Components.Authorization;

namespace ExpressMeals.WebUI.HttpService;

public class RefreshTokenService
{
    private readonly AuthenticationStateProvider _authProvider;
    private readonly IApiService _authService;

    public RefreshTokenService(AuthenticationStateProvider authProvider, IApiService authService)
    {
        _authProvider = authProvider;
        _authService = authService;
    }

    public async Task<string> TryRefreshToken()
    {
        var authState = await _authProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var exp = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
        var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
        var timeUtc = DateTime.UtcNow;
        var diff = expTime - timeUtc;
        if (diff.TotalMinutes <= 2)
            return await _authService.RefreshToken();
        return string.Empty;
    }
}