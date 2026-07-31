using System.ComponentModel.DataAnnotations;

namespace BoardApi.Dtos;

public record LoginRequest(
    [Required] string LoginId,
    [Required] string Password
);
