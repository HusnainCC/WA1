using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using WA1.Context;
using WA1.Repositories;
using WA1.UserInterface;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddDbContext<WA1dbcontext>(options =>
    options.UseInMemoryDatabase("WA1InMemoryDb"));

// Add AuthService
builder.Services.AddScoped<IAuthService, AuthRepo>();
builder.Services.AddScoped<IEmployee, UserRepo>();

var key = new byte[32]; 

string base64Secret = Convert.ToBase64String(key);
Console.WriteLine("Generated Secret Key: " + base64Secret);  

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:JwtIssuer"], 
            ValidAudience = builder.Configuration["JwtSettings:JwtAudience"],  
            IssuerSigningKey = new SymmetricSecurityKey(
                Convert.FromBase64String(base64Secret)) 
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($"Challenge: {context.ErrorDescription}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:4200") // Replace with your Angular app URL
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
