namespace BoardApi.Dtos;

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string UserId { get; set; }

    public required string Password { get; set; }

    public DateTime CreatedAt = DateTime.UtcNow;
}