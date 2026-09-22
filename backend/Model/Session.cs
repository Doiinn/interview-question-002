using System.ComponentModel.DataAnnotations.Schema;

namespace Example.Model;

[Table("sessions")]
public class Session
{
    [Column("id")]
    public string Id { get; set; } = "";
    [Column("user_id")]
    public int UserId { get; set; }
    [Column("expiry_date")]
    public string ExpiryDate { get; set; } = "";
}
