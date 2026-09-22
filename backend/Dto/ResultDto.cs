using System.Text.Json.Serialization;

namespace Example.Response;

public class ResultDto
{
    [JsonPropertyName("code")]
    public int Code { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; } = "";
}
