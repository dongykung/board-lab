namespace BoardApi.Exceptions;

public class InvalidCredentialsException : BoardApiException
{
    public override int StatusCode => StatusCodes.Status401Unauthorized;
    public override string Title => "Unauthorized";

    public InvalidCredentialsException() : base("Invalid login id or password.") { }
}
