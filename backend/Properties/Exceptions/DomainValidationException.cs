namespace BoardApi.Exceptions;

public class DomainValidationException : BoardApiException
{
    public DomainValidationException(string message) : base(message) {}

    public override int StatusCode => StatusCodes.Status400BadRequest;

    public override string Title => "Validation Failed";
}