using System.ComponentModel.DataAnnotations.Schema;

namespace Example.Model;

[Table("users")]
public class User
{
    [Column("id")]
    public int Id { get; set; }
    [Column("username")]

    public string UserName { get; set; } = "";
    [Column("password")]

    public string Password { get; set; } = "";
}
