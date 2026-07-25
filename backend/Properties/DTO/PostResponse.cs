namespace BoardApi.Dtos;

public record PostResponse(
    int Id,
    string Title,
    string Content,
    string AuthorName,
    int ViewCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt
)
{
    public static PostResponse FromEntity(Models.Post post) =>
        new(post.Id, post.Title, post.Content, post.AuthorName, post.ViewCount, post.CreatedAt, post.UpdatedAt);
}