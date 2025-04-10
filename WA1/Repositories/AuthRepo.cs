using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WA1.Context;
using WA1.Entities;
using WA1.Request;

public class AuthRepo : IAuthService
{
    private readonly WA1dbcontext _context;
    private readonly byte[] _jwtSecret;

    public AuthRepo(WA1dbcontext context, IConfiguration configuration)
    {
        _context = context;

        // Load the JWT Secret from configuration (make sure it's base64 encoded)
        var secretFromConfig = configuration["JwtSettings:JwtSecret"];

        if (string.IsNullOrEmpty(secretFromConfig))
        {
            throw new ArgumentNullException("JwtSecret", "JWT Secret must be configured in appsettings.json.");
        }

        try
        {
            // Convert the base64 encoded string into a byte array
            _jwtSecret = Convert.FromBase64String(secretFromConfig);

            // Validate that the key length is correct for HMACSHA256 (at least 256 bits or 32 bytes)
            if (_jwtSecret.Length <= 32)
            {
                throw new ArgumentException("JWT Secret must be at least 256 bits (32 bytes).");
            }
        }
        catch (FormatException ex)
        {
            // Improved exception handling: Detailed message
            throw new ArgumentException("JWT Secret is not a valid Base64 string. Please check the format in appsettings.json.", ex);
        }
    }


    // Registration
    public async Task<string> RegisterAsync(RegisterRequestobj request)
    {
        // Check if username already exists
        var existingUser = await _context.Employees
            .FirstOrDefaultAsync(e => e.Name == request.Username);

        if (existingUser != null)
        {
            // Username already exists
            return null;
        }

        // Hash the password before saving it
        var hashedPassword = HashPassword(request.Password);

        // Create a new employee
        var newEmployee = new Employee
        {
            Name = request.Username,
            Password = hashedPassword,  // Store the hashed password
        };

        // Save to the database
        await _context.Employees.AddAsync(newEmployee);
        await _context.SaveChangesAsync();

        // Return the JWT token after successful registration
        return GenerateJwtToken(newEmployee);
    }



    // Generate JWT Token
    private string GenerateJwtToken(Employee employee)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, employee.Name),
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),  // Token expires in 1 hour
            Issuer = "YourIssuer",  // Can be customized
            Audience = "YourAudience",  // Can be customized
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtSecret), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);  // Return the token as string
    }

    // Hash password using PBKDF2
    private string HashPassword(string password)
    {
        byte[] salt = new byte[16];
       
        // Hash the password with PBKDF2
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8  // 256 bits
        ));
    }

    // Verify entered password against stored hashed password
    private bool VerifyPassword(string enteredPassword, string storedPassword)
    {
        return storedPassword == HashPassword(enteredPassword);
    }

    // Get employee by username (for verification or additional functionality)
    public async Task<Employee> GetEmployeeByUsernameAsync(string username)
    {
        return await _context.Employees.FirstOrDefaultAsync(e => e.Name == username);
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        // Fetch employee from the database
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Name == username);

        // If employee doesn't exist
        if (employee == null)
        {
            // Log or return meaningful message
            throw new Exception("User does not exist. Please sign up first.");
        }

        // If password is incorrect
        if (!VerifyPassword(password, employee.Password))
        {
            throw new Exception("Incorrect password.");
        }

        // Generate JWT token for valid login
        return GenerateJwtToken(employee);
    }


}
