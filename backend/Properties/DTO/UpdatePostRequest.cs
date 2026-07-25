namespace BoardApi.Dtos;

public record UpdatePostRequest(
    string Title,
    string Content
);