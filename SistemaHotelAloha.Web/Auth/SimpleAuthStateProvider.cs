using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace SistemaHotelAloha.Web.Auth;

public class SimpleAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
    private ClaimsPrincipal _current;

    public SimpleAuthStateProvider()
    {
        _current = _anonymous;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => Task.FromResult(new AuthenticationState(_current));

    public void SignIn(string userName)
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) }, "Custom");
        _current = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void SignOut()
    {
        _current = _anonymous;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
