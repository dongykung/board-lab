namespace BoardApi.Dtos;

public record UserResponse(
    int Id,
    string Name,
    string LoginId,
    DateTime CreatedAt
)
{
    public static UserResponse FromEntity(Models.User user) =>
        new(user.Id, user.Name, user.LoginId, user.CreatedAt);
}
