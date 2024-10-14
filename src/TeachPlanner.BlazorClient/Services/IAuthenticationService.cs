using TeachPlanner.Shared.Contracts.Authentication;

namespace TeachPlanner.BlazorClient.Services;

public interface IAuthenticationService
{
    event Action<string?>? LoginChange;

    ValueTask<string> GetJwt();
    Task Login(LoginModel model);
    Task Register(RegisterModel model);
    Task Logout();
    Task DeleteAccount();
    Task Refresh();
}