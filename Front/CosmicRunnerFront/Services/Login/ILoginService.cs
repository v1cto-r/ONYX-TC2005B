using CosmicRunnerFront.Models;

namespace CosmicRunnerFront.Services.Login;

public interface ILoginService
{
    Task<int> LoginUser(string correo, string password);
}