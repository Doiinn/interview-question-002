using Example;
using Example.Dto;
using Example.Model;
using Example.Response;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

//for poc only, do not use this in production
const string SECRET_KEY = "your_super_secret_key_with_at_least_256_bits_for_security";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=example.db"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateActor = false,
        ValidateSignatureLast = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Validates tokens and populates HttpContext.User
app.UseAuthentication();

// Enforces policies like [Authorize]
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/", () => "Hello World!");

app.MapPost("/api/auth", async (UserDto user, AppDbContext db) =>
{
    var result = new ResultDto()
    {
        Code = (int)HttpStatusCode.ServiceUnavailable,
        Message = "Service unavailable"
    };

    //validate username rule
    if (user.UserName.Length < 6 || user.UserName.Length > 20 || user.UserName.Contains(' '))
    {
        result.Code = (int)HttpStatusCode.Unauthorized;
        result.Message = "Incorrect user or password (length)";

        return result;
    }

    //check existing user name
    var existed = await db.Users.FirstOrDefaultAsync(d => d.UserName == user.UserName);
    if (existed == null)
    {
        result.Code = (int)HttpStatusCode.Unauthorized;
        result.Message = "Incorrect user or password";

        return result;
    }

    //validate password rule
    if (user.Password.Length < 8 || user.Password.Length > 20 || user.Password.Contains(' '))
    {
        result.Code = (int)HttpStatusCode.Unauthorized;
        result.Message = "Incorrect user or password (length)";

        return result;
    }

    //verify password hash
    bool isPass = BCrypt.Net.BCrypt.Verify(user.Password, existed.Password);

    if (isPass)
    {
        //todo: create jwt token

        //for poc only, do not use this in production
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var expireInSeconds = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, existed.Id.ToString()),                                  // Subject (User ID)
            new Claim(JwtRegisteredClaimNames.Exp, expireInSeconds.ToString(), ClaimValueTypes.Integer64)  // Expire Time
        };

        var token = new JwtSecurityToken(
            issuer: "https://example.com",              // Where it came from
            audience: "https://example.com",            // Who can use it
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(1),    // 1 hour expiration
            signingCredentials: credentials
        );
        var handler = new JwtSecurityTokenHandler();
        string tokenString = handler.WriteToken(token);

        // Console.WriteLine(tokenString);
        result.Code = (int)HttpStatusCode.OK;
        result.Message = tokenString;
    }
    else
    {
        result.Code = (int)HttpStatusCode.Unauthorized;
        result.Message = "Incorrect user or password";
    }

    return result;
});

app.MapPost("/api/register", async (UserDto user, AppDbContext db) =>
{
    var result = new ResultDto()
    {
        Code = (int)HttpStatusCode.ServiceUnavailable,
        Message = "Service unavailable"
    };

    //validate username rule
    if (user.UserName.Length < 6 || user.UserName.Length > 20 || user.UserName.Contains(' '))
    {
        result.Code = (int)HttpStatusCode.Conflict;
        result.Message = "Must be 6-20 characters long and no space contain";

        return result;
    }

    //check existing user name
    var existed = await db.Users.FirstOrDefaultAsync(d => d.UserName == user.UserName);
    if (existed != null)
    {
        result.Code = (int)HttpStatusCode.Conflict;
        result.Message = "This Username is already taken";

        return result;
    }

    //check password rule
    if (user.Password.Length < 8 || user.Password.Length > 20 || user.Password.Contains(' '))
    {
        result.Code = (int)HttpStatusCode.Conflict;
        result.Message = "Must be 8-20 characters long and no space contain";

        return result;
    }

    //create new user and password into db
    var newUser = new User()
    {
        UserName = user.UserName,
        Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
    };
    db.Users.Add(newUser);

    await db.SaveChangesAsync();

    //created success
    result.Code = (int)HttpStatusCode.Created;
    result.Message = "Registration success";

    return result;
});

app.MapGet("/api/member",
    [Authorize] async ([FromHeader] string authorization, AppDbContext db) =>
    {
        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        {
            var scheme = headerValue.Scheme;
            var parameter = headerValue.Parameter;

            var handler = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler();
            var token = handler.ReadJsonWebToken(parameter);

            int userId = int.Parse(token.GetClaim("sub").Value);

            var existed = await db.Users.FindAsync(userId);
            if (existed != null)
            {
                Console.WriteLine(existed.UserName);
                return existed.UserName;
            }
        }

        return "";
    });

app.Run();
