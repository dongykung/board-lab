namespace BoardApi.Models;

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string LoginId { get; set; }

    public required string Password { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}