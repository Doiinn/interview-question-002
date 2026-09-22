using System.Text.Json.Serialization;

namespace Example.Dto;

public class UserDto
{
    [JsonPropertyName("username")]
    public string UserName { get; set; } = "";
    [JsonPropertyName("password")]
    public string Password { get; set; } = "";
}