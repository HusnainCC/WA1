using WA1.Entities;
using WA1.Request;

public interface IAuthService
{
    
    Task<string> RegisterAsync(RegisterRequestobj registerRequest);

    Task<string> LoginAsync(string username, string password);

    Task<Employee> GetEmployeeByUsernameAsync(string username);
}
