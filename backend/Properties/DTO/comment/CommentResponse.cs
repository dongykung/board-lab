using System.ComponentModel.DataAnnotations;

namespace BoardApi.Dtos;

public record CommentResponse(
    [Required] int CommentId,
    [Required] string Content,
    [Required] DateTime CreatedAt,
    [Required] DateTime UpdatedAt
);