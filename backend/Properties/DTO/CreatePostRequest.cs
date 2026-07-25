namespace BoardApi.Dtos;

public record CreatePostRequest(
    string Title,
    string Content,
    string AuthorName
);