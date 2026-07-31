namespace BoardApi.Exceptions;

public class DuplicateLoginIdException : BoardApiException
{
    public string LoginId { get; }
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string Title => "Conflict";

    public DuplicateLoginIdException(string loginId) : base($"LoginId '{loginId}' is already taken.")
    {
        LoginId = loginId;
    }
}
