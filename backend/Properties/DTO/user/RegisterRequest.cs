using System.ComponentModel.DataAnnotations;

namespace BoardApi.Dtos;

public record RegisterRequest(
    [Required, MaxLength(30)] string Name,
    [Required, MaxLength(30)] string LoginId,
    [Required, MinLength(8)] string Password
);
