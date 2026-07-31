using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BoardApi.Dtos;

public record PostResponse(
    int Id,
    string Title,
    string Content,
    string AuthorName,
    int userId,
    int ViewCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt
)
{
    public static PostResponse FromEntity(Models.Post post, Models.User user) =>
        new(post.Id, post.Title, post.Content, user.Name, user.Id, post.ViewCount, post.CreatedAt, post.UpdatedAt);
}