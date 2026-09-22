using Example;
using Example.Dto;
using Example.Model;
using Example.Response;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=example.db"));

var app = builder.Build();

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
    bool isPass =  BCrypt.Net.BCrypt.Verify(user.Password, existed.Password);

    if (isPass)
    {
        //todo: create jwt token
    }

    result.Code = isPass ? (int)HttpStatusCode.OK : (int)HttpStatusCode.Unauthorized;
    result.Message = isPass ? "Authentication success" : "Incorrect user or password";
    return result;
});

app.MapPost("/api/register", async (UserDto user, AppDbContext db) =>
{
    var result = new ResultDto()
    {
        Code = (int)HttpStatusCode.ServiceUnavailable,
        Message = "Service unavailable"
    };

    //check existing user name
    var existed = await db.Users.FirstOrDefaultAsync(d => d.UserName == user.UserName);
    if (existed != null)
    {
        result.Code = (int)HttpStatusCode.Conflict;
        result.Message = "This Username is already taken";

        return result;
    }

    //todo: check username rule
    //todo: check password rule

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


app.Run();
