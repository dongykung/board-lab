using System.ComponentModel.DataAnnotations;

namespace BoardApi.Dtos;

public record UpdateCommentRequest(
    [Required] string Content
);