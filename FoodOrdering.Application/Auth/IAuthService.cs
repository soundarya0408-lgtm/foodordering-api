using FoodOrdering.Application.Auth;

namespace FoodOrdering.Application.Auth;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
