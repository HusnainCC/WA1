using Microsoft.AspNetCore.Mvc;
using WA1.Request;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestobj request)
    {
        // Validate registration input
        if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Username and Password are required.");
        }

        // Register user (don't generate token here)
        var token = await _authService.RegisterAsync(request);

        // If token is null, it means registration failed (e.g., username already exists)
        if (token == null)
        {
            return Conflict("Username already exists.");
        }

        // Successfully registered, return success message (no token generated)
        return Ok("User registered successfully.");
    }


    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestobj request)
    {
        // Validate login input
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Username and Password are required.");
        }

        // Attempt to log in and retrieve a JWT token
        var token = await _authService.LoginAsync(request.Username, request.Password);

        // Handle invalid login credentials
        if (token == null)
        {
            return Unauthorized("Invalid credentials.");
        }

        // Return token upon successful login
        return Ok(new { Token = token });
    }
}
