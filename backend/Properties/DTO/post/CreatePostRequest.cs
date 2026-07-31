using System.ComponentModel.DataAnnotations;

namespace BoardApi.Dtos;

public record CreatePostRequest(
    [Required, MaxLength(100)] string Title,
    [Required] string Content
);