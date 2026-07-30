using System.ComponentModel.DataAnnotations;

namespace BoardApi.Dtos;

public record CreateCommentRequest(
    [Required] string Content
);
